﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MarkovSharpNetCore.Models;
using MarkovSharpNetCore.TokenisationStrategies;
using Newtonsoft.Json;

namespace MarkovSharpNetCore
{
    /// <summary>
    /// This class contains core functionality of the generic Markov model.
    /// Shouldn't be used directly, instead, extend GenericMarkov 
    /// and implement the IMarkovModel interface - this will allow you to 
    /// define overrides for SplitTokens and RebuildPhrase, which is generally
    /// all that should be needed for implementation of a new model type.
    /// </summary>
    /// <typeparam name="TPhrase"></typeparam>
    /// <typeparam name="TGram"></typeparam>
    public abstract class GenericMarkov<TPhrase, TGram> : IMarkovStrategy<TPhrase, TGram>
    {
        public GenericMarkov(int level = 2)
        {
            if (level < 1)
            {
                throw new ArgumentException("Invalid value: level must be a positive integer", nameof(level));
            }

            Model = new ConcurrentDictionary<SourceGrams<TGram>, List<TGram>>();
            SourceLines = new List<TPhrase>();
            Level = level;
            EnsureUniqueWalk = false;
        }

        // Dictionary containing the model data. The key is the N number of
        // previous words and value is a list of possible outcomes, given that key
        [JsonIgnore]
        public ConcurrentDictionary<SourceGrams<TGram>, List<TGram>> Model { get; set; }

        public List<TPhrase> SourceLines { get; set; }

        /// <summary>
        /// Defines how to split the phrase to ngrams
        /// </summary>
        /// <param name="phrase"></param>
        /// <returns></returns>
        public virtual IEnumerable<TGram> SplitTokens(TPhrase phrase)
        {
            throw new ArgumentException(
                "Please do not use GenericMarkov directly - instead, inherit from GenericMarkov and extend SplitTokens and RebuildPhrase methods. An interface IMarkovModel is provided for ease of use.");
        }

        /// <summary>
        /// Defines how to join ngrams back together to form a phrase
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public abstract TPhrase RebuildPhrase(IEnumerable<TGram> tokens);

        public abstract TGram GetTerminatorGram();

        public abstract TGram GetPrepadGram();

        /// <summary>
        /// Set to true to ensure that all lines generated are different and not same as the training data.
        /// This might not return as many lines as requested if genreation is exhausted and finds no new unique values.
        /// </summary>
        public bool EnsureUniqueWalk { get; set; }

        // The number of previous states for the model to to consider when 
        //suggesting the next state
        public int Level { get; private set; }

        public void Learn(IEnumerable<TPhrase> phrases, bool ignoreAlreadyLearnt = true)
        {
            if (ignoreAlreadyLearnt)
            {
                var newTerms = phrases.Where(s => !SourceLines.Contains(s));
                // For every sentence which hasnt already been learnt, learn it
                Parallel.ForEach(phrases, Learn);
            }
            else
            {
                // For every sentence, learn it
                Parallel.ForEach(phrases, Learn);
            }
        }

        public void Learn(TPhrase phrase)
        {
            if (phrase == null || phrase.Equals(default(TPhrase)))
            {
                return;
            }

            // Ignore particularly short sentences
            if (SplitTokens(phrase).Count() < Level)
            {
                return;
            }

            // Add it to the source lines so we can ignore it 
            // when learning in future
            if (!SourceLines.Contains(phrase))
            {
                SourceLines.Add(phrase);
            }

            // Split the sentence to an array of words
            var tokens = SplitTokens(phrase).ToArray();

            LearnTokens(tokens);

            var lastCol = new List<TGram>();
            for (var j = Level; j > 0; j--)
            {
                TGram previous;
                try
                {
                    previous = tokens[tokens.Length - j];
                    lastCol.Add(previous);
                }
                catch (IndexOutOfRangeException e)
                {
                    previous = GetPrepadGram();
                    lastCol.Add(previous);
                }
            }

            var finalKey = new SourceGrams<TGram>(lastCol.ToArray());
            AddOrCreate(finalKey, GetTerminatorGram());
        }

        private void LearnTokens(IReadOnlyList<TGram> tokens)
        {
            for (var i = 0; i < tokens.Count; i++)
            {
                var current = tokens[i];

                var previousCol = new List<TGram>();
                for (var j = Level; j > 0; j--)
                {
                    TGram previous;
                    try
                    {
                        if (i - j < 0)
                        {
                            previousCol.Add(GetPrepadGram());
                        }
                        else
                        {
                            previous = tokens[i - j];
                            previousCol.Add(previous);
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        previous = GetPrepadGram();
                        previousCol.Add(previous);
                    }
                }

                var key = new SourceGrams<TGram>(previousCol.ToArray());
                AddOrCreate(key, current);
            }
        }

        public void Retrain(int newLevel)
        {
            if (newLevel < 1)
            {
                throw new ArgumentException("Invalid argument - retrain level must be a positive integer",
                    nameof(newLevel));
            }

            Level = newLevel;

            // Empty the model so it can be rebuilt
            Model = new ConcurrentDictionary<SourceGrams<TGram>, List<TGram>>();

            Learn(SourceLines, false);
        }

        private object lockObj = new object();

        private void AddOrCreate(SourceGrams<TGram> key, TGram value)
        {
            lock (lockObj)
            {
                if (!Model.ContainsKey(key))
                {
                    Model.TryAdd(key, new List<TGram> {value});
                }
                else
                {
                    Model[key].Add(value);
                }
            }
        }

        public IEnumerable<TPhrase> Walk(int lines = 1, TPhrase seed = default(TPhrase))
        {
            if (seed == null)
            {
                seed = RebuildPhrase(new List<TGram>() {GetPrepadGram()});
            }

            if (lines < 1)
            {
                throw new ArgumentException("Invalid argument - line count for walk must be a positive integer",
                    nameof(lines));
            }

            var sentences = new List<TPhrase>();

            //for (var z = 0; z < lines; z++)k
            int genCount = 0;
            int created = 0;
            while (created < lines)
            {
                if (genCount == lines * 10)
                {
                    break;
                }
                var result = WalkLine(seed);
                if ((!EnsureUniqueWalk || !SourceLines.Contains(result)) &&
                    (!EnsureUniqueWalk || !sentences.Contains(result)))
                {
                    sentences.Add(result);
                    created++;
                    yield return result;
                }
                genCount++;
            }
        }

        private TPhrase WalkLine(TPhrase seed)
        {
            var arraySeed = PadArrayLow(SplitTokens(seed)?.ToArray());
            List<TGram> built = new List<TGram>();

            // Allocate a queue to act as the memory, which is n 
            // levels deep of previous words that were used
            var q = new Queue<TGram>(arraySeed);

            // If the start of the generated text has been seeded,
            // append that before generating the rest
            if (!seed.Equals(GetPrepadGram()))
            {
                built.AddRange(SplitTokens(seed));
            }

            while (built.Count < 1500)
            {
                // Choose a new word to add from the model
                //Logger.Info($"In Walkline loop: builtcount = {built.Count}");
                var key = new SourceGrams<TGram>(q.Cast<TGram>().ToArray());
                if (Model.ContainsKey(key))
                {
                    var chosen = Model[key].OrderBy(x => Guid.NewGuid()).FirstOrDefault();

                    q.Dequeue();
                    q.Enqueue(chosen);
                    built.Add(chosen);
                }
                else
                {
                    break;
                }
            }

            return RebuildPhrase(built);
        }

        // Returns any viable options for the next word based on
        // what was provided as input, based on the trained model.
        public List<TGram> GetMatches(TPhrase input)
        {
            var inputArray = SplitTokens(input).ToArray();
            if (inputArray.Count() > Level)
            {
                inputArray = inputArray.Skip(inputArray.Length - Level).ToArray();
            }
            else if (inputArray.Count() < Level)
            {
                inputArray = PadArrayLow(inputArray);
            }

            var key = new SourceGrams<TGram>(inputArray);
            var chosen = Model[key];
            return chosen;
        }

        // Pad out an array with empty strings from bottom up
        // Used when providing a seed sentence or word for generation
        private TGram[] PadArrayLow(TGram[] input)
        {
            if (input == null)
            {
                input = new List<TGram>().ToArray();
            }

            var splitCount = input.Length;
            if (splitCount > Level)
            {
                input = input.Skip(splitCount - Level).Take(Level).ToArray();
            }

            var p = new TGram[Level];
            int j = 0;
            for (int i = (Level - input.Length); i < (Level); i++)
            {
                p[i] = input[j];
                j++;
            }
            for (int i = Level - input.Length; i > 0; i--)
            {
                p[i - 1] = GetPrepadGram();
            }

            return p;
        }

        // Save the model to file for use later
        public void Save(string file)
        {
            var modelJson = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(file, modelJson);
        }

        // Load a model which has been saved
        public T Load<T>(string file, int level = 1) where T : IMarkovStrategy<TPhrase, TGram>
        {
            var model = JsonConvert.DeserializeObject<T>(File.ReadAllText(file));
            model.Retrain(level);

            return model;
        }
    }
}
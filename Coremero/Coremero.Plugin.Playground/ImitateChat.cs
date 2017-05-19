using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Coremero.Client;
using Coremero.Commands;
using Coremero.Context;
using Coremero.Messages;
using Coremero.Services;
using Coremero.Utilities;
using MarkovSharpNetCore.TokenisationStrategies;

namespace Coremero.Plugin.Playground
{
    public class ImitateChat : IPlugin
    {
        readonly Dictionary<string, StringMarkov> _models = new Dictionary<string, StringMarkov>();

        public ImitateChat(IMessageBus messageBus)
        {
            messageBus.Received += MessageBus_Received;
            foreach (string path in Directory.GetFiles(PathExtensions.ResourceDir, "*.*markov"))
            {
                try
                {
                    var tempMarkov = new StringMarkov().Load<StringMarkov>(path);
                    if (Path.GetExtension(path) == "usermarkov")
                    {
                        _userModels[ulong.Parse(Path.GetFileNameWithoutExtension(path))] = tempMarkov;
                    }
                    else
                    {
                        _models[Path.GetFileNameWithoutExtension(path)] = tempMarkov;
                    }

                }
                catch (Exception e)
                {
                    Log.Error($"Unable to load markov file: {path}");
                }
            }
        }

        private void MessageBus_Received(object sender, MessageReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Message.Text?.Trim()) || e.Message.Text.IsCommand())
            {
                return;
            }

            IChannel channel = e.Context?.Channel;
            IEntity entity = e.Context?.User as IEntity;
            if (channel != null)
            {
                if (e.Context.User?.Name != e.Context.OriginClient.Username)
                {
                    if (_models.ContainsKey(e.Context.Channel.Name))
                    {
                        _models[channel.Name].Learn(e.Message.Text);
                    }
                }
            }

            if (entity != null)
            {
                if (_userModels.ContainsKey(entity.ID))
                {
                    _userModels[entity.ID].Learn(e.Message.Text);
                }
                else
                {
                    StringMarkov markov = new StringMarkov();
                    _userModels.TryAdd(entity.ID, markov);
                    markov.Learn(e.Message.Text);
                }
            }
        }

        [Command("imichat")]
        public async Task<string> ImiChat(IInvocationContext context)
        {
            IBufferedChannel bufferedChannel = context.Channel as IBufferedChannel;
            if (bufferedChannel != null)
            {
                if (!_models.ContainsKey(bufferedChannel.Name))
                {
                    StringMarkov markov = new StringMarkov(1) { EnsureUniqueWalk = true };
                    DateTimeOffset offset = DateTimeOffset.UtcNow;
                    List<IBufferedMessage> messages = await bufferedChannel.GetMessagesAsync(offset, SearchDirection.Before, 5000);
                    markov.Learn(messages.Where(x => x.User.Name != context.OriginClient.Username && !string.IsNullOrEmpty(x.Text?.Trim()) && !x.Text.IsCommand()).Select(x => x.Text));
                    _models[bufferedChannel.Name] = markov;
                }

                return _models[context.Channel.Name].Walk(10).OrderByDescending(x => x.Length).Take(5).GetRandom();
            }
            throw new Exception("Not buffered channel.");
        }

        ConcurrentDictionary<ulong, StringMarkov> _userModels = new ConcurrentDictionary<ulong, StringMarkov>();

        [Command("fillusermarkovs", MinimumPermissionLevel = UserPermission.BotOwner)]
        public async Task<string> FillUserMarkovs(IInvocationContext context)
        {
            int linesLearned = 0;
            string botName = context.OriginClient.Username;
            foreach (var server in context.OriginClient.Servers)
            {
                Parallel.ForEach(server.Channels, channel =>
                {
                    IBufferedChannel bufferedChannel = channel as IBufferedChannel;
                    if (bufferedChannel != null && !bufferedChannel.Name.Contains("homero-dev"))
                    {
                        try
                        {
                            Parallel.ForEach(bufferedChannel.GetLatestMessagesAsync(15000).Result, (message =>
                            {
                                if (message.User.Name == botName || string.IsNullOrEmpty(message.Text.Trim()) ||
                                    message.Text.IsCommand())
                                {
                                    return;
                                }
                                IEntity entity = message.User as IEntity;
                                if (entity != null)
                                {
                                    if (!_userModels.ContainsKey(entity.ID))
                                    {
                                        _userModels.TryAdd(entity.ID, new StringMarkov(1) { EnsureUniqueWalk = true });
                                    }
                                    StringMarkov markov;
                                    _userModels.TryGetValue(entity.ID, out markov);
                                    markov?.Learn(message.Text);
                                    linesLearned += 1;
                                }
                            }));
                        }
                        catch
                        {
                            Log.Warn($"Could not read {bufferedChannel.Name}");
                        }
                    }
                });
            }

            return $"Identified {_userModels.Count} users. Learned {linesLearned} lines.";
        }

        [Command("realchat")]
        public string RealChat(IInvocationContext context)
        {
            List<IUser> users = context.Channel.Users?.GetRandom(8).ToList();
            Random rnd = new Random();
            StringBuilder sb = new StringBuilder();
            if (context.OriginClient.Features.HasFlag(ClientFeature.Markdown))
            {
                sb.AppendLine("```");
            }

            int lines = rnd.Next(4, 9);
            for (int i = 0; i < lines; i++)
            {
                IUser randomUser = users.GetRandom();
                IEntity entity = randomUser as IEntity;
                if (entity != null && _userModels.ContainsKey(entity.ID))
                {
                    sb.AppendLine($"{randomUser.Name}: {_userModels[entity.ID].Walk().First()}");
                }
            }

            if (context.OriginClient.Features.HasFlag(ClientFeature.Markdown))
            {
                sb.AppendLine("```");
            }
            return sb.ToString();
        }

        [Command("imiself")]
        public string ImiSelf(IInvocationContext context)
        {
            IEntity entity = context.User as IEntity;
            if (entity != null)
            {
                return _userModels[entity.ID].Walk().First();
            }
            throw new Exception("Not an entity.");
        }

        [Command("dumpmarkov", MinimumPermissionLevel = UserPermission.BotOwner)]
        public string DumpMarkovs()
        {
            int counter = 0;
            foreach (var keyValuePair in _models)
            {
                keyValuePair.Value.Save(Path.Combine(PathExtensions.ResourceDir, keyValuePair.Key + ".channelmarkov"));
                counter += 1;
            }
            foreach (var keyValuePair in _userModels)
            {
                keyValuePair.Value.Save(Path.Combine(PathExtensions.ResourceDir, keyValuePair.Key + ".usermarkov"));
                counter += 1;
            }
            return $"Dumped {counter} markovs to disk.";
        }
    }
}

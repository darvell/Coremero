![buildstatus](https://ci.appveyor.com/api/projects/status/github/darvell/Coremero?branch=master&svg=true)
![logo](https://raw.githubusercontent.com/darvell/coremero/master/coremero.png)

# Coremero
.NET Core chat bot that provides abstractions/interfaces to allow any chat service to become a client.
This bot is mostly a 'comedy' bot with a lot of silly commands for the members of an online community of jaded ex-minecraft players.

You can probably fork this and make your own silly joke commands if you're just looking for something to quickly get going for your own community.

## Supported

* Discord
* IRC (soon?)

## MyGet package feed

```
https://www.myget.org/F/coremero/api/v3/index.json
```

## Basic how-to on making a plugin

* Create a .NET Standard >=1.6 DLL.
* Reference the Coremero nuget package from the MyGet feed above.
* In your project create a standard class.
* Make this class implement the ```IPlugin``` interface. This interface serves only to tag.
* Create a method that returns either any object that implements ToString() or an IMessage (use Coremero.Messages.Message if building a very custom response).
* Annotate this method with the ```Coremero.Command``` attribute and feed it the command name.
* Your end result should look something like this:

```
public class TestPlugin : IPlugin
{
    [Command("hello")]
    public string SayHello()
    {
       return "hello!";
    }
}
```

* Check the existing plugins on how to get invocation contexts, message objects, et al.

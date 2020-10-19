using System;

namespace CoAndContravariance
{
    public interface ICommand
    {
        string Victim { get; }
    }

    public class SimpleCommand : ICommand
    {
        public SimpleCommand(string victim)
        {
            Victim = victim;
        }

        public string Victim { get; }
    }

    public interface IFollowCommand: ICommand
    {
        string Place { get; }
    }

    public class FollowCommand: IFollowCommand
    {
        public FollowCommand(string victim, string place)
        {
            Victim = victim;
            Place = place;
        }

        public string Victim { get; }
        public string Place { get; }
    }

    public interface IMurderCommand : IFollowCommand
    {
        string Weapon { get; }
    }

    public class MurderCommand : IMurderCommand
    {
        public MurderCommand(string victim, string place, string weapon)
        {
            Victim = victim;
            Place = place;
            Weapon = weapon;
        }

        public string Victim { get; }
        public string Place { get; }
        public string Weapon { get; set; }
    }

    public interface IContravariantKiller<in TCommand> where TCommand : ICommand
    {
        string Name { get; }
        public void Terrorize(TCommand command);
    }

    public abstract class Killer<TCommand>: IContravariantKiller<TCommand> where TCommand : ICommand
    {
        protected Killer(string name)
        {
            Name = name;
        }

        public string Name { get; }
        
        public abstract void Terrorize(TCommand command);
    }

    public class WatchingKiller : Killer<ICommand>
    {
        public override void Terrorize(ICommand command)
        {
            Console.WriteLine($"Following victim {command.Victim}");
        }

        public WatchingKiller(string name) : base(name)
        {
        }
    }



    public class FollowerKiller: Killer<IFollowCommand>
    {
        public override void Terrorize(IFollowCommand command)
        {
            Console.WriteLine($"Following victim {command.Victim} until {command.Place}");
        }

        public FollowerKiller(string name) : base(name)
        {
        }
    }


    public class MurderKiller : Killer<IMurderCommand>
    {
        public override void Terrorize(IMurderCommand command)
        {
            Console.WriteLine($"Following victim {command.Victim} until {command.Place} and then kill by {command.Weapon}");
        }

        public MurderKiller(string name) : base(name)
        {
        }
    }

    public class Program
    {

        public static void PerformKill<TCommand>(IContravariantKiller<TCommand> killer, TCommand command) where TCommand: ICommand
        {
            Console.WriteLine("Doing kill");
            Console.WriteLine($"Killer {killer.Name}");

            killer.Terrorize(command);
        }
        public static void Main(string[] args)
        {
            var simpleCommand = new SimpleCommand("Vasya");
            var simpleKiller = new WatchingKiller("Chikatilo");

            PerformKill<ICommand>(simpleKiller, simpleCommand);

            Console.WriteLine(new string('-', 30));

            var followCommand = new FollowCommand("Kolya", "Park");
            var followingKiller = new FollowerKiller("Walking Chikatilo");

            PerformKill<IFollowCommand>(simpleKiller, followCommand);

            Console.WriteLine(new string('-', 30));

            var murderCommand = new MurderCommand("Kolya", "Park", "Knife");
            var murderKiller = new MurderKiller("Potroshitel");

            PerformKill<IMurderCommand>(murderKiller, murderCommand);

            Console.WriteLine(new string('-', 30));

            PerformKill<IMurderCommand>(followingKiller, murderCommand);

            // Console.WriteLine(new string('-', 30));

            // PerformKill<IFollowCommand>(murderKiller, followCommand); - error
        }

        // Вывод - более простой киллер может выполнить более сложную задачу как умеет, но более опытный не возьмется за простую задачу
        // Преследователь может взять заказ на убийство, но убийца не может преследовать
    }
}

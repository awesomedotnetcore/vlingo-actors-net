using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Vlingo
{
    public class Directory
    {
        private readonly ConcurrentDictionary<Address, Actor>[] _maps;

        public int Count
        {
            get { return _maps.Sum(map => map.Count); }
        }

        internal Directory()
        {
            _maps = BuildMaps(20).ToArray();
        }

        public void Dump()
        {
            foreach (var map in _maps)
            {
                foreach (var actor in map.Values)
                {
                    var address = actor.Address;
                    var parent = actor.InternalParent() == null ? new Address(0, "NONE") : actor.InternalParent().Address;
                    Console.WriteLine($"DIR: DUMP: ACTOR: {address} PARENT: {parent}");
                }
            }
        }

        public bool IsRegistered(Address address)
        {
            return _maps[MapIndex(address)].ContainsKey(address);
        }

        public void Register(Address address, Actor actor)
        {
            if (IsRegistered(address))
            {
                throw new Exception("The actor address is already registered: " + address);
            }
            _maps[MapIndex(address)][address] = actor;
        }

        public Actor Remove(Address address)
        {
            _maps[MapIndex(address)].TryRemove(address, out var actor); // what if cannot delete
            return actor;
        }

        private IEnumerable<ConcurrentDictionary<Address, Actor>> BuildMaps(int count)
        {
            return Enumerable.Range(0, count).Select(t =>
            {
                return new ConcurrentDictionary<Address, Actor>(); // initialCapacity: 16, loadFactor: 0.75f, concurrencyLevel: 16
                // TODO: base this on scheduler/dispatcher
            });
        }

        private int MapIndex(Address address)
        {
            return Math.Abs(address.GetHashCode() % _maps.Length);
        }
    }
}
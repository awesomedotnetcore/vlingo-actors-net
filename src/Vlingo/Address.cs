using System;
using System.Threading;

namespace Vlingo
{
    public sealed class Address
        : IComparable<Address>
    {
        private static int _nextId = 1;
        public readonly int Id;
        public readonly string Name;

        internal Address(string name)
            : this(Interlocked.Increment(ref _nextId), name)
        {
        }

        internal Address(int reservedId, string name)
        {
            Id = reservedId;
            Name = name ?? reservedId.ToString();
        }

        public int CompareTo(Address other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }
            if (ReferenceEquals(null, other))
            {
                return 1;
            }
            return Id.CompareTo(other.Id);
        }

        public static Address From(string name)
        {
            return new Address(name);
        }


        public override int GetHashCode()
        {
            return Id;
        }

        public override string ToString()
        {
            return $"Address[{Id}, name={Name ?? "(none)"}]";
        }

        internal static Address From(int reservedId, string name)
        {
            return new Address(reservedId, name);
        }

        internal static int TestNextIdValue()
        {
            return Interlocked.Increment(ref _nextId); // for test only
        }
    }
}
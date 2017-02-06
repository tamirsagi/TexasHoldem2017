using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListTest
{
    class Program
    {
        static void Main(string[] args)
        {
            List<A> a = new List<A>();
            a.Add(new A());
            a.Add(new A());
            a.Add(new A());
            a.Add(new A());

            a[1] = null;

            Console.WriteLine(a.Count);
            a[1].print();
        }

    }
    class A
    {
        public int g;
        public A()
        {
            g = 1;
        }
        public void print()
        {
            Console.WriteLine("g = "+g+"\n");
        }
    }
}

using System;

namespace QuickCheck {

    public abstract class A {
        public int fld1{get;}
        public int fld2;

        public A(int a, int b) {
            fld1 = a;
            fld2 = b;
        }
    }

    public class B : A {
        public int fld3;
        public int fld4;

        public B(int a, int b) : base (a, b) {
            
        }
    }


    class Program {
        static void Main(string[] args) {
            
            B testB = new B(1,3) { 
                fld2=2, fld4 = 4
            };
            
            Console.WriteLine(testB.fld3);
        }
    }
}

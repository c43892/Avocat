using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swift
{
    public class AsyncCalleeChain<T>
    {
        List<object> callee = new List<object>();

        public void Add(Func<T, IEnumerator> a)
        {
            callee.Add(a);
        }

        public void Del(Func<T, IEnumerator> a)
        {
            callee.Remove(a);
        }

        public void Add(Action<T> a)
        {
            callee.Add(a);
        }

        public void Del(Action<T> a)
        {
            callee.Remove(a);
        }

        public IEnumerator Invoke(T p)
        {
            var arr = callee.ToArray();
            for (var i = 0; i < arr.Length; i++)
            {
                if (arr[i] is Action<T>)
                    (arr[i] as Action<T>)(p);
                else
                    yield return (arr[i] as Func<T, IEnumerator>)(p);
            }
        }
    }

    public class AsyncCalleeChain<T1, T2>
    {
        List<object> callee = new List<object>();

        public void Add(Func<T1, T2, IEnumerator> a)
        {
            callee.Add(a);
        }

        public void Del(Func<T1, T2, IEnumerator> a)
        {
            callee.Remove(a);
        }

        public void Add(Action<T1, T2> a)
        {
            callee.Add(a);
        }

        public void Del(Action<T1, T2> a)
        {
            callee.Remove(a);
        }

        public IEnumerator Invoke(T1 p1, T2 p2)
        {
            var arr = callee.ToArray();
            for (var i = 0; i < arr.Length; i++)
            {
                if (arr[i] is Action<T1, T2>)
                    (arr[i] as Action<T1, T2>)(p1, p2);
                else
                    yield return (arr[i] as Func<T1, T2, IEnumerator>)(p1, p2);
            }
        }
    }

    public class AsyncCalleeChain<T1, T2, T3>
    {
        List<object> callee = new List<object>();

        public void Add(Func<T1, T2, T3, IEnumerator> a)
        {
            callee.Add(a);
        }

        public void Del(Func<T1, T2, T3, IEnumerator> a)
        {
            callee.Remove(a);
        }

        public void Add(Action<T1, T2, T3> a)
        {
            callee.Add(a);
        }

        public void Del(Action<T1, T2, T3> a)
        {
            callee.Remove(a);
        }

        public IEnumerator Invoke(T1 p1, T2 p2, T3 p3)
        {
            var arr = callee.ToArray();
            for (var i = 0; i < arr.Length; i++)
            {
                if (arr[i] is Action<T1, T2, T3>)
                    (arr[i] as Action<T1, T2, T3>)(p1, p2, p3);
                else
                    yield return (arr[i] as Func<T1, T2, T3, IEnumerator>)(p1, p2, p3);
            }
        }
    }

    public class AsyncCalleeChain<T1, T2, T3, T4>
    {
        List<object> callee = new List<object>();

        public void Add(Func<T1, T2, T3, T4, IEnumerator> a)
        {
            callee.Add(a);
        }

        public void Add(Action<T1, T2, T3, T4> a)
        {
            callee.Add(a);
        }

        public void Del(Action<T1, T2, T3, T4> a)
        {
            callee.Remove(a);
        }

        public IEnumerator Invoke(T1 p1, T2 p2, T3 p3, T4 p4)
        {
            var arr = callee.ToArray();
            for (var i = 0; i < arr.Length; i++)
            {
                if (arr[i] is Action<T1, T2, T3, T4>)
                    (arr[i] as Action<T1, T2, T3, T4>)(p1, p2, p3, p4);
                else
                    yield return (arr[i] as Func<T1, T2, T3, T4, IEnumerator>)(p1, p2, p3, p4);
            }
        }
    }

    public class AsyncCalleeChain<T1, T2, T3, T4, T5>
    {
        List<object> callee = new List<object>();

        public void Add(Func<T1, T2, T3, T4, T5, IEnumerator> a)
        {
            callee.Add(a);
        }

        public void Add(Action<T1, T2, T3, T4, T5> a)
        {
            callee.Add(a);
        }

        public void Del(Action<T1, T2, T3, T5> a)
        {
            callee.Remove(a);
        }

        public IEnumerator Invoke(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
        {
            var arr = callee.ToArray();
            for (var i = 0; i < arr.Length; i++)
            {
                if (arr[i] is Action<T1, T2, T3, T4, T5>)
                    (arr[i] as Action<T1, T2, T3, T4, T5>)(p1, p2, p3, p4, p5);
                else
                    yield return (arr[i] as Func<T1, T2, T3, T4, T5, IEnumerator>)(p1, p2, p3, p4, p5);
            }
        }
    }
}

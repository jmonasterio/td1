using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/* Usage

    RunInfo info;
 foreach (MyScript s in scriptsToRun)
 {
     info = s.MyAction().ParallelCoroutine(); //Optionally specify a group here
 }
 while(info.count > 0) yield return null;

    */
namespace Assets.Scripts
{
    using System.Collections;
    using UnityEngine;

    public class RunInfo
    {
        public int count;
        public static Dictionary<string, RunInfo> runners = new Dictionary<string, RunInfo>();
    }

    public class ParallelSupport : MonoBehaviour
    {
        public static ParallelSupport instance;

        void Awake()
        {
            instance = this;
        }
    }

    public static class ParallelCoroutineExt
    {
        public static RunInfo ParallelCoroutine(this IEnumerator coroutine, string group = "default")
        {
            if (!RunInfo.runners.ContainsKey(group))
            {
                RunInfo.runners[group] = new RunInfo();
            }
            var ri = RunInfo.runners[group];
            ri.count++;
            ParallelSupport.instance.StartCoroutine(DoParallel(coroutine, ri));
            return ri;
        }

        static IEnumerator DoParallel(IEnumerator coroutine, RunInfo ri)
        {
            yield return ParallelSupport.instance.StartCoroutine(coroutine);
            ri.count--;
        }
    }
}

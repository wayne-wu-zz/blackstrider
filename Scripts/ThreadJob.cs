using UnityEngine;
using System.Collections;
using System.Threading;

public class ThreadJob {

    private bool _isDone = false;
    private object _Handle = new object();
    private Thread _thread = null;

    public bool isDone
    {
        get
        {
            bool tmp;
            lock (_Handle)
            {
                tmp = _isDone;
            }
            return tmp;
        }
        set
        {
            lock (_Handle)
            {
                _isDone = value;
            }
        }
    }

	public virtual void Start () {
        _thread = new Thread(Run);
        _thread.Start();
	}


    public virtual void Abort()
    {
        _thread.Abort();
    }

    public virtual bool Update()
    {
        if (isDone)
        {
            OnFinished();
            return true;
        }
        return false;
    }

    protected virtual void ThreadFunction() { }

    protected virtual void OnFinished() { }


    public IEnumerator WaitFor()
    {
        while (!Update())
        {
            yield return null;
        }
    }

    public virtual void Run()
    {
        ThreadFunction();
        isDone = true;
    }
	
}

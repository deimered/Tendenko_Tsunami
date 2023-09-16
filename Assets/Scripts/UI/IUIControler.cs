using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IUIControler
{
    public event EventHandler OnPause;
    public void Activate();
    public void Deactivate();
}

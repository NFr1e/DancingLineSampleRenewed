using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IResettable
{
    /// <summary>
    /// 需要手动在RespawnAttributes中订阅
    /// </summary>
    void NoteArgs();
    void ResetArgs();
}

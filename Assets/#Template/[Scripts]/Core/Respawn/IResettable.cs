using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IResettable
{
    /// <summary>
    /// ��Ҫ�ֶ���RespawnAttributes�ж���
    /// </summary>
    void NoteArgs();
    void ResetArgs();
}

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TeaCollection", menuName = "Tea/Tea Collection")]
public class TeaCollection : ScriptableObject
{
   public List<TeaData> allTeas = new List<TeaData>();
}
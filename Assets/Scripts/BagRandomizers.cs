using System;
using UnityEngine;
using crass;

[Serializable]
public class TransformBag : BagRandomizer<Transform> {}

[Serializable]
public class EnemyBag : BagRandomizer<EnemyCategory> {}

[Serializable]
public class ColorBag : BagRandomizer<MagicColor> {}

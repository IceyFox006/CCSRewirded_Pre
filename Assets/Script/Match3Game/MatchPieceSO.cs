using UnityEngine;

[CreateAssetMenu(fileName = "MatchPieceSO", menuName = "Scriptable Objects/Match3/Piece")]
public class MatchPieceSO : ScriptableObject
{
    [Header("Data"), SerializeField]
        private ElementSO _element;
    [Header("Data"), SerializeField] 
        private EffectTag[] _breakEffects;

    [Header("Visuals"), SerializeField]
        private Sprite _sprite;


}

public enum EffectTag
{
    BREAK_ROW = 001,
    BREAK_COLUMN = 002,
    BREAK_ALL_OF_ELEMENT = 003,
}

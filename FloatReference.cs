[System.Serializable]
public class FloatReference
{
    public bool useConstant = true;
    public float constantValue;
    public FloatVariable variable; // a ScriptableObject

    public float Value => useConstant ? constantValue : variable.Value;
}

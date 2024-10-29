namespace GPU.Placeholders.Processing.UnitsOfWork.Structs;

public struct FormulaProcessorStruct
{
    public char[] RFIDateArrayBuffer;
    public char[] Formula_USD_ArrayBuffer;
    public char[] FormulaArrayBuffer; 

    public FormulaProcessorStruct(
        char[] rfidateArrayBuffer, 
        char[] formulausdArrayBuffer,
        char[] formulaArrayBuffer 
    )
    {
        this.RFIDateArrayBuffer = rfidateArrayBuffer;
        this.Formula_USD_ArrayBuffer = formulausdArrayBuffer;
        this.FormulaArrayBuffer = formulaArrayBuffer;
    }
}
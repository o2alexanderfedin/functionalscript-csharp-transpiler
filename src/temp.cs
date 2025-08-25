global using FSArray = FSValue[];


public interface IFSVisitor<R>
{
    R VisitNumber(double f64);
    R VisitArray(FSArray arr);

}

public struct FSValue
{
    private object ptr;

    public FSValue(double f64)
    {
        ptr = f64;
    }

    public FSValue(FSArray arr)
    {
        ptr = arr;
    }

    public R Apply<R>(IFSVisitor<R> v)
    {
        return ptr switch
        {
            double d => v.VisitNumber(d),
            FSArray a => v.VisitArray(a),
            _ => throw new ArgumentException(nameof(ptr))
        };
    }

    public static FSValue operator +(FSValue left, FSValue right)
    {
        throw new NotImplementedException();
    } 
}

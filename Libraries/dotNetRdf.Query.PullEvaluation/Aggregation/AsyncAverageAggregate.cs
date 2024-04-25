using VDS.RDF;
using VDS.RDF.Nodes;
using VDS.RDF.Query.Expressions;

namespace dotNetRdf.Query.PullEvaluation.Aggregation;

public class AsyncAverageAggregate : IAsyncAggregation
{
    private readonly ISparqlExpression _expression;
    private readonly bool _distinct;
    private readonly PullEvaluationContext _context;
    private bool _error;
    private long _count = 0;
    private decimal _decimalSum = 0.0m;
    private float _floatSum = 0.0f;
    private double _doubleSum = 0.0d;
    private SparqlNumericType _maxtype = SparqlNumericType.NaN;
    private SparqlNumericType _numtype = SparqlNumericType.NaN;
    private readonly HashSet<INode>? _values;

    public AsyncAverageAggregate(ISparqlExpression valueExpression, bool distinct, string variableName,
        PullEvaluationContext context)
    {
        _expression = valueExpression;
        _distinct = distinct;
        VariableName = variableName;
        _context = context;
        if (_distinct) _values = new HashSet<INode>();
    }
    public string VariableName { get; }

    public INode? Value
    {
        get
        {
            if (_error) return null;
            if (_count == 0) return new LongNode(0);
            switch (_numtype)
            {
                case SparqlNumericType.Integer:
                case SparqlNumericType.Decimal:
                    return new DecimalNode(_decimalSum / _count);
                case SparqlNumericType.Float:
                    return new FloatNode(_floatSum / _count);
                case SparqlNumericType.Double:
                    return new DoubleNode(_doubleSum / _count);
                default:
                    return null;
            }
        }
    }

    public void Start()
    {
        _count = 0;
        _decimalSum = 0.0m;
        _floatSum = 0.0f;
        _doubleSum = 0.0d;
    }

    public bool Accept(ExpressionContext expressionContext)
    {
        if (_error) return false;
        IValuedNode? tmp = _expression.Accept(_context.ExpressionProcessor, _context, expressionContext);
        if (tmp == null) { 
            _error = true;
            return false;
        }

        if (_distinct && _values != null)
        {
            if (!_values.Add(tmp))
            {
                return true;
            }
        }
        _numtype = tmp.NumericType;
        if (_numtype == SparqlNumericType.NaN)
        {
            _error = true;
            return false;
        }
        
        // Track the Numeric Type
        if ((int)_numtype > (int)_maxtype)
        {
            _maxtype = _numtype;
        }

        // Increment the Totals based on the current Numeric Type
        switch (_maxtype)
        {
            case SparqlNumericType.Integer:
            case SparqlNumericType.Decimal:
                _decimalSum += tmp.AsDecimal();
                _floatSum += tmp.AsFloat();
                _doubleSum += tmp.AsDouble();
                break;
            case SparqlNumericType.Float:
                _floatSum += tmp.AsFloat();
                _doubleSum += tmp.AsDouble();
                break;
            case SparqlNumericType.Double:
                _doubleSum += tmp.AsDouble();
                break;
        }

        _count++;
        return true;
    }

    public void End()
    {
    }
}
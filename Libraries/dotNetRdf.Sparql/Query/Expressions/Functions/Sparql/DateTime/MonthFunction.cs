/*
// <copyright>
// dotNetRDF is free and open source software licensed under the MIT License
// -------------------------------------------------------------------------
// 
// Copyright (c) 2009-2021 dotNetRDF Project (http://dotnetrdf.org/)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is furnished
// to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// </copyright>
*/

namespace VDS.RDF.Query.Expressions.Functions.Sparql.DateTime
{
    /// <summary>
    /// Represents the SPARQL MONTH() Function.
    /// </summary>
    public class MonthFunction
        : XPath.DateTime.MonthFromDateTimeFunction
    {
        /// <summary>
        /// Creates a new SPARQL YEAR() Function.
        /// </summary>
        /// <param name="expr">Argument Expression.</param>
        public MonthFunction(ISparqlExpression expr)
            : base(expr) { }

        /// <summary>
        /// Gets the Functor of this Expression.
        /// </summary>
        public override string Functor
        {
            get
            {
                return SparqlSpecsHelper.SparqlKeywordMonth;
            }
        }

        /// <summary>
        /// Gets the String representation of this Expression.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return SparqlSpecsHelper.SparqlKeywordMonth + "(" + InnerExpression.ToString() + ")";
        }

        /// <summary>
        /// Transforms the Expression using the given Transformer.
        /// </summary>
        /// <param name="transformer">Expression Transformer.</param>
        /// <returns></returns>
        public override ISparqlExpression Transform(IExpressionTransformer transformer)
        {
            return new MonthFunction(transformer.Transform(InnerExpression));
        }

        public override TResult Accept<TResult, TContext, TBinding>(ISparqlExpressionProcessor<TResult, TContext, TBinding> processor, TContext context, TBinding binding)
        {
            return processor.ProcessMonthFunction(this, context, binding);
        }

        public override T Accept<T>(ISparqlExpressionVisitor<T> visitor)
        {
            return visitor.VisitMonthFunction(this);
        }
    }
}

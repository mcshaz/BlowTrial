using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

//http://stackoverflow.com/questions/25256257/finding-reference-to-dbfunction-within-expression-tree-and-replacing-with-a-diff
//What an answer - REALLY thankful to SVICK
namespace BlowTrial.Helpers
{
    class ReplaceMethodCallVisitor : ExpressionVisitor
    {
        readonly MethodInfo methodToReplace;
        readonly Func<IReadOnlyList<Expression>, Expression> replacementFunction;

        public ReplaceMethodCallVisitor(
            MethodInfo methodToReplace,
            Func<IReadOnlyList<Expression>, Expression> replacementFunction)
        {
            this.methodToReplace = methodToReplace;
            this.replacementFunction = replacementFunction;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method == methodToReplace)
            {
                var replacement = replacementFunction(
                    node.Arguments.Select(StripNullable).ToList());

                if (replacement.Type != node.Type)
                    return Expression.Convert(replacement, node.Type);
            }

            return base.VisitMethodCall(node);
        }

        private static Expression StripNullable(Expression e)
        {

            if (e is UnaryExpression unaryExpression && e.NodeType == ExpressionType.Convert
                && unaryExpression.Operand.Type == Nullable.GetUnderlyingType(e.Type))
            {
                return unaryExpression.Operand;
            }

            return e;
        }
    }
}

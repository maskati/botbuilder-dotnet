﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace Microsoft.Bot.Builder.Expressions.Parser
{


    /// <summary>
    /// Parser to turn strings into an <see cref="Expression"/>.
    /// </summary>
    public class ExpressionEngine : IExpressionParser
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="lookup">If present delegate to lookup evaluation information from type string.</param>
        public ExpressionEngine(EvaluatorLookup lookup = null)
        {
            _lookup = lookup ?? BuiltInFunctions.Lookup;
        }

        private EvaluatorLookup _lookup;


        /// <summary>
        /// Parse the input into an expression.
        /// </summary>
        /// <param name="expression">Expression to parse.</param>
        /// <returns>Expresion tree.</returns>
        public Expression Parse(string expression)
        {
            return new ExpressionTransformer(_lookup).Transform(AntlrParse(expression));
        }

        private IParseTree AntlrParse(string expression)
        {
            var inputStream = new AntlrInputStream(expression);
            var lexer = new ExpressionLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new ExpressionParser(tokenStream);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(ExpressionErrorListener.Instance);
            parser.BuildParseTree = true;
            return parser.expression();
        }

        private class ExpressionTransformer : ExpressionBaseVisitor<Expression>
        {
            public ExpressionTransformer(EvaluatorLookup lookup)
            {
                _lookup = lookup;
            }

            private readonly EvaluatorLookup _lookup;

            private Expression MakeExpression(string type, params Expression[] children)
            {
                return Expression.MakeExpression(type, _lookup(type), children);
            }

            public Expression Transform(IParseTree context)
            {
                return Visit(context);
            }

            private IEnumerable<Expression> ProcessArgsList(ExpressionParser.ArgsListContext context)
            {
                if (context != null)
                {
                    foreach (var expression in context.expression())
                    {
                        yield return Visit(expression);
                    }
                }
            }

            public override Expression VisitUnaryOpExp([NotNull] ExpressionParser.UnaryOpExpContext context)
            {
                var unaryOperationName = context.GetChild(0).GetText();
                var operand = Visit(context.expression());
                return MakeExpression(unaryOperationName, operand);
            }

            public override Expression VisitBinaryOpExp([NotNull] ExpressionParser.BinaryOpExpContext context)
            {
                var binaryOperationName = context.GetChild(1).GetText();
                var left = Visit(context.expression(0));
                var right = Visit(context.expression(1));
                return MakeExpression(binaryOperationName, left, right);
            }

            public override Expression VisitFuncInvokeExp([NotNull] ExpressionParser.FuncInvokeExpContext context)
            {
                var parameters = ProcessArgsList(context.argsList()).ToList();

                //if context.primaryExpression() is idAtom --> normal function
                if (context.primaryExpression() is ExpressionParser.IdAtomContext idAtom)
                {
                    var functionName = idAtom.GetText();
                    return MakeExpression(functionName, parameters.ToArray());
                }

                // TODO: We really should interpret this as a function with a namespace and not an accessor with a function.  Should also loop over them.
                //if context.primaryExpression() is memberaccessExp --> accessor
                if (context.primaryExpression() is ExpressionParser.MemberAccessExpContext memberAccessExp)
                {
                    var instance = Visit(memberAccessExp.primaryExpression());
                    var functionName = memberAccessExp.IDENTIFIER().GetText();
                    parameters.Insert(0, instance);
                    return MakeExpression(functionName, parameters.ToArray());
                }

                throw new Exception("This format is wrong.");
            }

            public override Expression VisitIdAtom([NotNull] ExpressionParser.IdAtomContext context)
            {
                Expression result;
                var symbol = context.GetText();
                if (symbol == "false")
                {
                    result = Expression.ConstantExpression(false);
                }
                else if (symbol == "true")
                {
                    result = Expression.ConstantExpression(true);
                }
                else
                {
                    result = MakeExpression(ExpressionType.Accessor, Expression.ConstantExpression(symbol));
                }
                return result;
            }

            public override Expression VisitIndexAccessExp([NotNull] ExpressionParser.IndexAccessExpContext context)
            {
                var instance = Visit(context.primaryExpression());
                var index = Visit(context.expression());
                return MakeExpression(ExpressionType.Element, instance, index );
            }

            public override Expression VisitMemberAccessExp([NotNull] ExpressionParser.MemberAccessExpContext context)
            {
                var instance = Visit(context.primaryExpression());
                return MakeExpression(ExpressionType.Accessor, Expression.ConstantExpression(context.IDENTIFIER().GetText()), instance);
            }

            public override Expression VisitNumericAtom([NotNull] ExpressionParser.NumericAtomContext context)
            {
                if (int.TryParse(context.GetText(), out var intValue))
                    return Expression.ConstantExpression(intValue);

                if (double.TryParse(context.GetText(), out var doubleValue))
                    return Expression.ConstantExpression(doubleValue);

                throw new Exception($"{context.GetText()} is not a number.");
            }

            public override Expression VisitParenthesisExp([NotNull] ExpressionParser.ParenthesisExpContext context) => Visit(context.expression());

            public override Expression VisitStringAtom([NotNull] ExpressionParser.StringAtomContext context) => Expression.ConstantExpression(Regex.Unescape(context.GetText().Trim('\'')));
        }

    }
}
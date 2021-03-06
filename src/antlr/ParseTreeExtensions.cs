﻿// --------------------------------------------------------------------------------
// Copyright (c) 2014, XLR8 Development
// --------------------------------------------------------------------------------
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// --------------------------------------------------------------------------------

using System;

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace tsql2pgsql.antlr
{
    public static class ParseTreeExtensions
    {
        /// <summary>
        /// Returns true if the child has a parent of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="child"></param>
        /// <returns></returns>
        public static bool HasParent<T>(this IParseTree child)
            where T : IParseTree
        {
            return FindParent<T>(child) != null;
        }

        /// <summary>
        /// Returns the first parent of type T.
        /// Returns null if no type is found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="child"></param>
        /// <returns></returns>
        public static T FindParent<T>(this IParseTree child)
            where T : IParseTree
        {
            if (child.Parent == null)
                return default(T);
            else if (child.Parent is T)
                return (T) child.Parent;
            else
                return FindParent<T>(child.Parent);
        }

        /// <summary>
        /// Returns the left most token.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static IToken LeftMostToken(this IParseTree context)
        {
            if (context == null)
            {
                return null;
            }
            else if (context is TerminalNodeImpl)
            {
                var terminalNode = (TerminalNodeImpl)context;
                return terminalNode.Symbol;
            }
            else
            {
                for (int ii = 0; ii < context.ChildCount; ii++)
                {
                    var token = LeftMostToken(context.GetChild(ii));
                    if (token != null)
                    {
                        return token;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Returns the right most token.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static IToken RightMostToken(this IParseTree context)
        {
            if (context == null)
            {
                return null;
            }
            else if (context is TerminalNodeImpl)
            {
                var terminalNode = (TerminalNodeImpl)context;
                return terminalNode.Symbol;
            }
            else
            {
                for (int ii = context.ChildCount - 1; ii >= 0; ii--)
                {
                    var token = RightMostToken(context.GetChild(ii));
                    if (token != null)
                    {
                        return token;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Returns the rightmost leaf node.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IParseTree RightMostChild(this IParseTree context)
        {
            if (context.ChildCount == 0)
                return context;
            return RightMostChild(context.GetChild(context.ChildCount - 1));
        }

        /// <summary>
        /// Gets the range.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        public static string ToRangeString(this IParseTree tree)
        {
            if (tree is TerminalNodeImpl)
            {
                var terminalNode = (TerminalNodeImpl)tree;
                return
                    terminalNode.Symbol.Line + "." +
                    terminalNode.Symbol.Column;
            }
            else if (tree is ParserRuleContext)
            {
                var ruleContext = (ParserRuleContext)tree;
                return
                    ruleContext.Start.Line + "." +
                    ruleContext.Start.Column + " -> " +
                    ruleContext.Stop.Line + "." +
                    (ruleContext.Stop.Column + ruleContext.Stop.Text.Length - ruleContext.Start.Column);
            }

            throw new InvalidOperationException();
        }

        public static bool IsParentChain<T>(this IParseTree node) where T : IParseTree
        {
            return node.Parent is T;
        }

        public static bool IsParentChain<T1, T2>(this IParseTree node)
            where T1 : IParseTree
            where T2 : IParseTree
        {
            return
                node.Parent is T1 &&
                node.Parent.Parent is T2;
        }

        public static bool IsParentChain<T1, T2, T3>(this IParseTree node)
            where T1 : IParseTree
            where T2 : IParseTree
        {
            return
                node.Parent is T1 &&
                node.Parent.Parent is T2 &&
                node.Parent.Parent.Parent is T3;
        }


    }
}

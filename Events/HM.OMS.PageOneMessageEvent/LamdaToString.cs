// Copyright 2016-2040 Nino Crudele
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#region Usings

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

#endregion

namespace HM.OMS.PageOneMessageConsole
{
    public class LamdaToString
    {
        public void Execute()
        {
            string[] names = {"Huan", "Long", "Loc"};
            Expression<Func<Product, bool>> expr = p => p.Description == "test" && names.Contains(p.Name);
            string name = GetPropertyName(expr);

            Console.Read();
        }

        public string GetPropertyName(Expression<Func<Product, bool>> expr)
        {
            var mexpr = (BinaryExpression) expr.Body;

            var member = (MemberExpression) expr.Body;
            var property = (PropertyInfo) member.Member;
            return property.Name;
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
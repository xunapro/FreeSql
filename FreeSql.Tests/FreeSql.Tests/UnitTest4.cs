﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Xunit;
using static FreeSql.Tests.UnitTest1;

namespace FreeSql.Tests
{
    public class UnitTest4
    {

        public record ts_record(DateTime Date, int TemperatureC, int TemperatureF, string Summary)
        {
            public ts_record parent { get; set; }
        }
        public record ts_record_dto(DateTime Date, int TemperatureC, string Summary);

        [Fact]
        public void LeftJoinNull01()
        {
            var fsql = g.sqlite;

            fsql.Delete<ts_record>().Where("1=1").ExecuteAffrows();
            fsql.Insert(new ts_record(DateTime.Now, 1, 2, "123")).ExecuteAffrows();
            var fores = fsql.Select<ts_record>().ToList();
            var fores_dtos1 = fsql.Select<ts_record>().ToList<ts_record_dto>();
            var fores_dtos2 = fsql.Select<ts_record>().ToList(a => new ts_record_dto(a.Date, a.TemperatureC, a.Summary));



            fsql.Delete<leftjoin_null01>().Where("1=1").ExecuteAffrows();
            fsql.Delete<leftjoin_null02>().Where("1=1").ExecuteAffrows();

            var item = new leftjoin_null01 { name = "xx01" };
            fsql.Insert(item).ExecuteAffrows();

            var sel1 = fsql.Select<leftjoin_null01, leftjoin_null02>()
                .LeftJoin((a, b) => a.id == b.null01_id)
                .First((a, b) => new
                {
                    a.id,
                    a.name,
                    id2 = (Guid?)b.id,
                    time2 = (DateTime?)b.time
                });
            Assert.Null(sel1.id2);
            Assert.Null(sel1.time2);
        }

        class leftjoin_null01
        {
            public Guid id { get; set; }
            public string name { get; set; }
        }
        class leftjoin_null02
        {
            public Guid id { get; set; }
            public Guid null01_id { get; set; }
            public DateTime time { get; set; }
        }


        [Fact]
        public void TestHzyTuple()
        {
            var xxxhzytuple = g.sqlite.Select<Templates, TaskBuild>()
                    .LeftJoin(w => w.t1.Id2 == w.t2.TemplatesId)
                    .Where(w => w.t1.Code == "xxx" && w.t2.OptionsEntity03 == true)
                    .OrderBy(w => w.t1.AddTime)
                    .ToSql();

            var xxxhzytupleGroupBy = g.sqlite.Select<Templates, TaskBuild>()
                    .LeftJoin(w => w.t1.Id2 == w.t2.TemplatesId)
                    .Where(w => w.t1.Code == "xxx" && w.t2.OptionsEntity03 == true)
                    .GroupBy(w => new { w.t1 })
                    .OrderBy(w => w.Key.t1.AddTime)
                    .ToSql(w => w.Key );

        }
    }
}

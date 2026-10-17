using Microsoft.EntityFrameworkCore;
using RaceDay.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace RaceDay.Tests.Helpers
{
    public static class TestDbHelper
    {
        public static RaceDayDbContext GetInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new RaceDayDbContext(options);
        }
    }
}

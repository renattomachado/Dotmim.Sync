﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Dotmim.Sync.Data;

namespace Dotmim.Sync.Core.Builders
{

    /// <summary>
    /// Helper for create a table
    /// </summary>
    public interface IDbBuilderTableHelper
    {
        DmTable TableDescription { get; set; }
        bool NeedToCreateTable(DbBuilderOption builderOption);
        void CreateTable();
        void CreatePrimaryKey();
        void CreateForeignKeyConstraints();
        string CreateTableScriptText();
        string CreatePrimaryKeyScriptText();
        string CreateForeignKeyConstraintsScriptText();
    }
}

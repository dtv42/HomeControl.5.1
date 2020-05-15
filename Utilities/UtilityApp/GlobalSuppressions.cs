// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlobalSuppressions.cs" company="DTV-Online">
//   Copyright(c) 2020 Dr. Peter Trimmel. All rights reserved.
// </copyright>
// <license>
//   Licensed under the MIT license. See the LICENSE file in the project root for more information.
// </license>
// <created>13-5-2020 13:53</created>
// <author>Peter Trimmel</author>
// --------------------------------------------------------------------------------------------------------------------

// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "<Pending>", Scope = "member", Target = "~P:UtilityApp.Models.AppSettings.StringArray")]
[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "<Pending>", Scope = "member", Target = "~P:UtilityApp.Models.AppSettings.BooleanArray")]
[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "<Pending>", Scope = "member", Target = "~P:UtilityApp.Models.AppSettings.IntegerArray")]
[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "<Pending>", Scope = "member", Target = "~P:UtilityApp.Models.AppSettings.LongArray")]
[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "<Pending>", Scope = "member", Target = "~P:UtilityApp.Models.AppSettings.FloatArray")]
[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "<Pending>", Scope = "member", Target = "~P:UtilityApp.Models.AppSettings.DoubleArray")]
[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "<Pending>", Scope = "member", Target = "~P:UtilityApp.Models.AppSettings.DecimalArray")]
[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "<Pending>", Scope = "member", Target = "~P:UtilityApp.Models.AppSettings.DateTimeArray")]
[assembly: SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "<Pending>", Scope = "member", Target = "~P:UtilityApp.Models.AppSettings.DateTimeOffsetArray")]
[assembly: SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>", Scope = "member", Target = "~M:UtilityApp.Commands.LogCommand.OnExecute~System.Int32")]
[assembly: SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>", Scope = "member", Target = "~M:UtilityApp.Commands.AsyncCommand.OnExecuteAsync(System.Threading.CancellationToken)~System.Threading.Tasks.Task{System.Int32}")]
[assembly: SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>", Scope = "member", Target = "~M:UtilityApp.Commands.PropertyCommand.OnExecute~System.Int32")]
[assembly: SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>", Scope = "member", Target = "~M:UtilityApp.Commands.RootCommand.OnExecute~System.Int32")]
[assembly: SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>", Scope = "member", Target = "~M:UtilityApp.Commands.SettingsCommand.OnExecute~System.Int32")]
[assembly: SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>", Scope = "member", Target = "~M:UtilityApp.Commands.TestdataCommand.OnExecute~System.Int32")]
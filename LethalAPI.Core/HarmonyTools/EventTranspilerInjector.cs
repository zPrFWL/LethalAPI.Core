﻿// -----------------------------------------------------------------------
// <copyright file="EventTranspilerInjector.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.HarmonyTools;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using Events.Features;
using Events.Interfaces;

using static AccessTools;

using OpCodes = System.Reflection.Emit.OpCodes;

/// <summary>
/// Provides methods for injecting events in various transpilers.
/// </summary>
// ReSharper disable InconsistentNaming
public static class EventTranspilerInjector
{
    private static List<TypeInfo>? types;

    /// <summary>
    /// Injects a deniable event into a transpiler add the given index, as well as any prefix args defined in <see cref="prefixInstructions"/>.
    /// </summary>
    /// <param name="instructions">The original method instructions.</param>
    /// <param name="generator">The ILGenerator in use.</param>
    /// <param name="index">The index to inject the instructions at.</param>
    /// <param name="prefixInstructions">Optional instructions that can be inject prior to the event constructor, that allow loading the stack with parameters for the constructor.</param>
    /// <typeparam name="T">The <see cref="IDeniableEvent"/> to inject.</typeparam>
    public static void InjectDeniableEvent<T>(ref IEnumerable<CodeInstruction> instructions, ref ILGenerator generator, int index, List<CodeInstruction>? prefixInstructions = null)
        where T : IDeniableEvent
    {
        if (instructions is not List<CodeInstruction> list)
        {
            list = instructions.ToList();
        }

        types ??= typeof(Log).Assembly.DefinedTypes
            .Where(x => x.FullName?.StartsWith("LethalAPI.Core.Events.Handlers") ?? false).ToList();

        PropertyInfo? propertyInfo = null;
        foreach (TypeInfo type in types)
        {
            propertyInfo = type
                .GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .FirstOrDefault(pty => pty.PropertyType == typeof(Event<T>));

            if (propertyInfo is not null)
            {
                break;
            }
        }

        if (propertyInfo is null)
        {
            Log.Warn($"Could not find an acceptable event handler for event {typeof(T).FullName}! No injection will occur for this event.");
            return;
        }

        LocalBuilder local = generator.DeclareLocal(typeof(T));
        Label rtn = generator.DefineLabel();

        List<CodeInstruction> opcodes = new()
        {
            // TEventArgs ev = new()
            new(OpCodes.Callvirt),
            new(OpCodes.Newobj, GetDeclaredConstructors(typeof(T))[0]),
            new(OpCodes.Dup),
            new(OpCodes.Dup),
            new(OpCodes.Stloc_S, local),
            new(OpCodes.Call, PropertyGetter(propertyInfo.DeclaringType, propertyInfo.Name)),
            new(OpCodes.Call, PropertyGetter(typeof(Event<T>), nameof(Event<T>.InvokeSafely))),

            // Handlers.{Handler}.{Event}.InvokeSafely(ev)

            // if (!ev.IsAllowed)
            //   return
            new(OpCodes.Callvirt, PropertyGetter(typeof(T), nameof(IDeniableEvent.IsAllowed))),
            new(OpCodes.Brfalse, rtn),
            new(OpCodes.Ret),
        };
        if (prefixInstructions is { Count: > 0 })
        {
            list.InsertRange(index, prefixInstructions);
            index += prefixInstructions.Count;
        }

        list.InsertRange(index, opcodes);
        index += opcodes.Count + 1;
        list[index] = list[index].WithLabels();
    }
}
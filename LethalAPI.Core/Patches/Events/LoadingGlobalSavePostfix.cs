﻿// -----------------------------------------------------------------------
// <copyright file="LoadingGlobalSavePostfix.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Patches.Events;

using LethalAPI.Core.Events.Attributes;
using LethalAPI.Core.Events.EventArgs;
using LethalAPI.Core.Events.Handlers;

#pragma warning disable SA1402

/// <summary>
///     Patches the <see cref="Server.LoadingSave"/> event.
/// </summary>
[HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.Start))]
[EventPatch(typeof(Server), nameof(Server.LoadingSave))]
internal static class LoadingGlobalSavePostfix
{
    [HarmonyPostfix]
    private static void Postfix()
    {
        Server.LoadingSave.InvokeSafely(new LoadingSaveEventArgs("LCGeneralSaveData", LoadedItem.LastSelectedSave));
    }
}

/// <summary>
///     Patches the <see cref="Server.LoadingSave"/> event.
/// </summary>
[HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.SpawnUnlockable))]
[EventPatch(typeof(Server), nameof(Server.LoadingSave))]
internal static class SpawnUnlockablePostfix
{
    [HarmonyPostfix]
    private static void Postfix(StartOfRound __instance)
    {
        Server.LoadingSave.InvokeSafely(new LoadingSaveEventArgs(GameNetworkManager.Instance.currentSaveFileName, LoadedItem.SpawnUnlockable));
    }
}

/// <summary>
///     Patches the <see cref="Server.LoadingSave"/> event.
/// </summary>
[HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.LoadUnlockables))]
[EventPatch(typeof(Server), nameof(Server.LoadingSave))]
internal static class LoadUnlockablesPostfix
{
    [HarmonyPostfix]
    private static void Postfix(StartOfRound __instance)
    {
        Server.LoadingSave.InvokeSafely(new LoadingSaveEventArgs(GameNetworkManager.Instance.currentSaveFileName, LoadedItem.LoadUnlockables));
    }
}

/// <summary>
///     Patches the <see cref="Server.LoadingSave"/> event.
/// </summary>
[HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.LoadShipGrabbableItems))]
[EventPatch(typeof(Server), nameof(Server.LoadingSave))]
internal static class LoadShipGrabbableItemsPostfix
{
    [HarmonyPostfix]
    private static void Postfix(StartOfRound __instance)
    {
        Server.LoadingSave.InvokeSafely(new LoadingSaveEventArgs(GameNetworkManager.Instance.currentSaveFileName, LoadedItem.LoadShipGrabbableItems));
    }
}

/// <summary>
///     Patches the <see cref="Server.LoadingSave"/> event.
/// </summary>
[HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.SetTimeAndPlanetToSavedSettings))]
[EventPatch(typeof(Server), nameof(Server.LoadingSave))]
internal static class SetTimeAndPlanetPostfix
{
    [HarmonyPostfix]
    private static void Postfix(StartOfRound __instance)
    {
        Server.LoadingSave.InvokeSafely(new LoadingSaveEventArgs(GameNetworkManager.Instance.currentSaveFileName, LoadedItem.SetTimeAndPlanetToSavedSettings));
    }
}
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace Blocky.Signs;

[StaticConstructorOnStartup]
public class Init
{
    static Init()
    {
        Harmony harmony = new Harmony("Blocky.Signs");
        harmony.PatchAll();
        Patch__BillDoors__Museum.HandlePatch(harmony);

        if( ModConfig.Settings.fixFramesCategory ){
            fixFramesCategory();
        }
    }

    private static readonly MethodInfo m_ResolveDesignators = AccessTools.Method(typeof(DesignationCategoryDef), "ResolveDesignators");
    private static readonly FastInvokeHandler _ResolveDesignators = MethodInvoker.GetHandler(m_ResolveDesignators);

    private static readonly FieldInfo f_resolvedDesignators = AccessTools.Field(typeof(DesignationCategoryDef), "resolvedDesignators");

    public static void fixFramesCategory(){
        var list = DefDatabase<ThingDef>
            .AllDefsListForReading
            .FindAll((ThingDef x) => 
                    x?.thingClass != null
                    && (x.thingClass == typeof(Building_Frame))
                    && ((BuildableDef)x).designationCategory != null
                    );

        DesignationCategoryDef blockyCat = DefDatabase<DesignationCategoryDef>.GetNamed("Blocky", false);

        bool moved = false;
        foreach( var def in list ){
            if( def.designationCategory != blockyCat ){
                var list2 = (List<Designator>)f_resolvedDesignators.GetValue(def.designationCategory);
                if( list2 != null ){
                    // remove them from "Storage" menu
                    list2.RemoveAll((Designator x) => x is Designator_Build && ((Designator_Place)(Designator_Build)x).PlacingDef == def);
                }
                def.designationCategory = blockyCat;
                moved = true;
            }
        }

        if( moved ){
            _ResolveDesignators(blockyCat);
            Core.Utils.ArchitectMenu_ClearCache();
        }
    }
}

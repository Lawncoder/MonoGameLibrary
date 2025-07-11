using System;
using System.Collections.Generic;
using nkast.Aether.Physics2D.Dynamics;

namespace Mario.Helpers;

public class CollisionAssistant
{
    public static Category CategoryFromLayers(params CollisionLayers[] layers)
    {
        Category category = Category.None;
        
        foreach (CollisionLayers layer in layers)
        {
            category += (int)layer;
        }
        
        return category;
    }

   

    public static bool CategoryInCategories(CollisionLayers layers, Category category)
    {
        return ((int)layers & (int)category) != 0;
    }
}
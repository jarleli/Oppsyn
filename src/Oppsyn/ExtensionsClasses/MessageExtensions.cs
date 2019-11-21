using Oppsyn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oppsyn.ExtensionsClasses
{
    public static class MessageExtensions
    {
        public static bool BlockHasTypeLinkOrHasChildWithTypeLink(this Block[] blocks)
        {
            return blocks.Any(b => b.Type == "link") ? true :
                blocks.Any(b => b.Elements.BlockElementHasTypeLinkOrHasChildWithTypeLink());
        }

        public static bool BlockElementHasTypeLinkOrHasChildWithTypeLink(this BlockElement[] blocks)
        {
            return blocks.Any(b => b.Type == "link")
                ? true
                : blocks.Any(b => b.Elements.BlockElementElementHasTypeLinkOrHasChildWithTypeLink());

        }
        public static bool BlockElementElementHasTypeLinkOrHasChildWithTypeLink(this ElementElementClass[] blocks)
        {
            return blocks.Any(b => b.Type == "link");
        }


    }
}


namespace Stateless {
    
    /// <summary>
    ///   A class for looking up localized strings, etc.
    /// </summary>
  
    internal class StateConfigurationResources {
        
     
        /// <summary>
        ///   Looks up a localized string similar to Permit() (and PermitIf()) require that the destination state is not equal to the source state. To accept a trigger without changing state, use either Ignore() or PermitReentry()..
        /// </summary>
        internal static string SelfTransitionsEitherIgnoredOrReentrant {
            get {
                return "Permit() (and PermitIf()) require that the destination state is not equal to the source state. To accept a trigger without changing state, use either Ignore() or PermitReentry().";
            }
        }
    }
}

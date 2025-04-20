
namespace Stateless {
    using System;
    using System.Reflection;
    
    
    /// <summary>
    ///    A class for looking up localized strings, etc.
    /// </summary>
    public class StateMachineResources {
        

        /// <summary>
        ///    Looks up a localized string similar to Parameters for the trigger &apos;{0}&apos; have already been configured..
        /// </summary>
        public static string CannotReconfigureParameters {
            get {
                return "Parameters for the trigger '{0}' have already been configured.";
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to No valid leaving transitions are permitted from state &apos;{1}&apos; for trigger &apos;{0}&apos;. Consider ignoring the trigger..
        /// </summary>
        public static string NoTransitionsPermitted {
            get {
                return "No valid leaving transitions are permitted from state '{1}' for trigger '{0}'. Consider ignoring the trigger.";
            }
        }
        
        /// <summary>
        ///    Looks up a localized string similar to Trigger &apos;{0}&apos; is valid for transition from state &apos;{1}&apos; but a guard conditions are not met. Guard descriptions: &apos;{2}&apos;..
        /// </summary>
        public static string NoTransitionsUnmetGuardConditions {
            get {
                return "Trigger '{0}' is valid for transition from state '{1}' but a guard conditions are not met. Guard descriptions: '{2}'.";
            }
        }
    }
}

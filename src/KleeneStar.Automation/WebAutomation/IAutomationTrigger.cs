using System;
using System.Collections.Generic;
using System.Linq;

namespace KleeneStar.Automation.WebAutomation
{
    /// <summary>
    /// A trigger an automation rule can react to.
    /// </summary>
    public interface IAutomationTrigger
    {
        /// <summary>
        /// Gets the key the rule stores, e.g. <c>transition.executed</c>.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Gets the i18n key of the trigger's label.
        /// </summary>
        string Label { get; }

        /// <summary>
        /// Gets the i18n key describing what the trigger parameter means, or <see langword="null"/>
        /// for a trigger that takes none.
        /// </summary>
        string Parameter { get; }

        /// <summary>
        /// Gets a value indicating whether the trigger is evaluated by the scheduler rather than
        /// raised by an event.
        /// </summary>
        bool Scheduled => false;
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Articy.Unity;
using Articy.Unity.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Articy.Underchoices
{
    
    
    public class Post_Template : Dialogue, IDialogue, IPropertyProvider
    {
        
        [SerializeField()]
        private ArticyValuePost_TemplateTemplate mTemplate = new ArticyValuePost_TemplateTemplate();
        
        private static Articy.Underchoices.Templates.Post_TemplateTemplateConstraint mConstraints = new Articy.Underchoices.Templates.Post_TemplateTemplateConstraint();
        
        public Articy.Underchoices.Templates.Post_TemplateTemplate Template
        {
            get
            {
                return mTemplate.GetValue();
            }
            set
            {
                mTemplate.SetValue(value);
            }
        }
        
        public static Articy.Underchoices.Templates.Post_TemplateTemplateConstraint Constraints
        {
            get
            {
                return mConstraints;
            }
        }
        
        protected override void CloneProperties(object aClone, Articy.Unity.ArticyObject aFirstClassParent)
        {
            Post_Template newClone = ((Post_Template)(aClone));
            if ((Template != null))
            {
                newClone.Template = ((Articy.Underchoices.Templates.Post_TemplateTemplate)(Template.CloneObject(newClone, aFirstClassParent)));
            }
            base.CloneProperties(newClone, aFirstClassParent);
        }
        
        public override bool IsLocalizedPropertyOverwritten(string aProperty)
        {
            return base.IsLocalizedPropertyOverwritten(aProperty);
        }
        
        #region property provider interface
        public override void setProp(string aProperty, object aValue)
        {
            if (aProperty.Contains("."))
            {
                Template.setProp(aProperty, aValue);
                return;
            }
            base.setProp(aProperty, aValue);
        }
        
        public override Articy.Unity.Interfaces.ScriptDataProxy getProp(string aProperty)
        {
            if (aProperty.Contains("."))
            {
                return Template.getProp(aProperty);
            }
            return base.getProp(aProperty);
        }
        #endregion
    }
}

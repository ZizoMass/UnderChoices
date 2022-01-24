//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Articy.Underchoices;
using Articy.Unity;
using Articy.Unity.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Articy.Underchoices.Features
{
    
    
    [Serializable()]
    public class DefaultBasicCharacterFeatureFeature : IArticyBaseObject, IPropertyProvider
    {
        
        [SerializeField()]
        private Single mAge;
        
        [SerializeField()]
        private String mSpecies;
        
        [SerializeField()]
        private String mBornIn;
        
        [SerializeField()]
        private Sex mSex = new Sex();
        
        [SerializeField()]
        private String mOccupation;
        
        [SerializeField()]
        private String mAccent;
        
        [SerializeField()]
        private String mPersonality;
        
        [SerializeField()]
        private String mAppearance;
        
        [SerializeField()]
        private UInt64 mOwnerId;
        
        [SerializeField()]
        private UInt32 mOwnerInstanceId;
        
        public Single Age
        {
            get
            {
                return mAge;
            }
            set
            {
                var oldValue = mAge;
                mAge = value;
                Articy.Unity.ArticyDatabase.ObjectNotifications.ReportChanged(OwnerId, OwnerInstanceId, "DefaultBasicCharacterFeature.Age", oldValue, mAge);
            }
        }
        
        public String Unresolved_Species
        {
            get
            {
                return mSpecies;
            }
        }
        
        public String Species
        {
            get
            {
                return Articy.Unity.ArticyTextExtension.Resolve(this, mSpecies);
            }
            set
            {
                var oldValue = mSpecies;
                mSpecies = value;
                Articy.Unity.ArticyDatabase.ObjectNotifications.ReportChanged(OwnerId, OwnerInstanceId, "DefaultBasicCharacterFeature.Species", oldValue, mSpecies);
            }
        }
        
        public String Unresolved_BornIn
        {
            get
            {
                return mBornIn;
            }
        }
        
        public String BornIn
        {
            get
            {
                return Articy.Unity.ArticyTextExtension.Resolve(this, mBornIn);
            }
            set
            {
                var oldValue = mBornIn;
                mBornIn = value;
                Articy.Unity.ArticyDatabase.ObjectNotifications.ReportChanged(OwnerId, OwnerInstanceId, "DefaultBasicCharacterFeature.BornIn", oldValue, mBornIn);
            }
        }
        
        public Sex Sex
        {
            get
            {
                return mSex;
            }
            set
            {
                var oldValue = mSex;
                mSex = value;
                Articy.Unity.ArticyDatabase.ObjectNotifications.ReportChanged(OwnerId, OwnerInstanceId, "DefaultBasicCharacterFeature.Sex", oldValue, mSex);
            }
        }
        
        public String Unresolved_Occupation
        {
            get
            {
                return mOccupation;
            }
        }
        
        public String Occupation
        {
            get
            {
                return Articy.Unity.ArticyTextExtension.Resolve(this, mOccupation);
            }
            set
            {
                var oldValue = mOccupation;
                mOccupation = value;
                Articy.Unity.ArticyDatabase.ObjectNotifications.ReportChanged(OwnerId, OwnerInstanceId, "DefaultBasicCharacterFeature.Occupation", oldValue, mOccupation);
            }
        }
        
        public String Unresolved_Accent
        {
            get
            {
                return mAccent;
            }
        }
        
        public String Accent
        {
            get
            {
                return Articy.Unity.ArticyTextExtension.Resolve(this, mAccent);
            }
            set
            {
                var oldValue = mAccent;
                mAccent = value;
                Articy.Unity.ArticyDatabase.ObjectNotifications.ReportChanged(OwnerId, OwnerInstanceId, "DefaultBasicCharacterFeature.Accent", oldValue, mAccent);
            }
        }
        
        public String Unresolved_Personality
        {
            get
            {
                return mPersonality;
            }
        }
        
        public String Personality
        {
            get
            {
                return Articy.Unity.ArticyTextExtension.Resolve(this, mPersonality);
            }
            set
            {
                var oldValue = mPersonality;
                mPersonality = value;
                Articy.Unity.ArticyDatabase.ObjectNotifications.ReportChanged(OwnerId, OwnerInstanceId, "DefaultBasicCharacterFeature.Personality", oldValue, mPersonality);
            }
        }
        
        public String Unresolved_Appearance
        {
            get
            {
                return mAppearance;
            }
        }
        
        public String Appearance
        {
            get
            {
                return Articy.Unity.ArticyTextExtension.Resolve(this, mAppearance);
            }
            set
            {
                var oldValue = mAppearance;
                mAppearance = value;
                Articy.Unity.ArticyDatabase.ObjectNotifications.ReportChanged(OwnerId, OwnerInstanceId, "DefaultBasicCharacterFeature.Appearance", oldValue, mAppearance);
            }
        }
        
        public UInt64 OwnerId
        {
            get
            {
                return mOwnerId;
            }
            set
            {
                mOwnerId = value;
            }
        }
        
        public UInt32 OwnerInstanceId
        {
            get
            {
                return mOwnerInstanceId;
            }
            set
            {
                mOwnerInstanceId = value;
            }
        }
        
        private void CloneProperties(object aClone, Articy.Unity.ArticyObject aFirstClassParent)
        {
            Articy.Underchoices.Features.DefaultBasicCharacterFeatureFeature newClone = ((Articy.Underchoices.Features.DefaultBasicCharacterFeatureFeature)(aClone));
            newClone.Age = Age;
            newClone.Species = Unresolved_Species;
            newClone.BornIn = Unresolved_BornIn;
            newClone.Sex = Sex;
            newClone.Occupation = Unresolved_Occupation;
            newClone.Accent = Unresolved_Accent;
            newClone.Personality = Unresolved_Personality;
            newClone.Appearance = Unresolved_Appearance;
            newClone.OwnerId = OwnerId;
        }
        
        public object CloneObject(object aParent, Articy.Unity.ArticyObject aFirstClassParent)
        {
            Articy.Underchoices.Features.DefaultBasicCharacterFeatureFeature clone = new Articy.Underchoices.Features.DefaultBasicCharacterFeatureFeature();
            CloneProperties(clone, aFirstClassParent);
            return clone;
        }
        
        public virtual bool IsLocalizedPropertyOverwritten(string aProperty)
        {
            return false;
        }
        
        #region property provider interface
        public void setProp(string aProperty, object aValue)
        {
            if ((aProperty == "Age"))
            {
                Age = System.Convert.ToSingle(aValue);
                return;
            }
            if ((aProperty == "Species"))
            {
                Species = System.Convert.ToString(aValue);
                return;
            }
            if ((aProperty == "BornIn"))
            {
                BornIn = System.Convert.ToString(aValue);
                return;
            }
            if ((aProperty == "Sex"))
            {
                Sex = ((Sex)(aValue));
                return;
            }
            if ((aProperty == "Occupation"))
            {
                Occupation = System.Convert.ToString(aValue);
                return;
            }
            if ((aProperty == "Accent"))
            {
                Accent = System.Convert.ToString(aValue);
                return;
            }
            if ((aProperty == "Personality"))
            {
                Personality = System.Convert.ToString(aValue);
                return;
            }
            if ((aProperty == "Appearance"))
            {
                Appearance = System.Convert.ToString(aValue);
                return;
            }
        }
        
        public Articy.Unity.Interfaces.ScriptDataProxy getProp(string aProperty)
        {
            if ((aProperty == "Age"))
            {
                return new Articy.Unity.Interfaces.ScriptDataProxy(Age);
            }
            if ((aProperty == "Species"))
            {
                return new Articy.Unity.Interfaces.ScriptDataProxy(Species);
            }
            if ((aProperty == "BornIn"))
            {
                return new Articy.Unity.Interfaces.ScriptDataProxy(BornIn);
            }
            if ((aProperty == "Sex"))
            {
                return new Articy.Unity.Interfaces.ScriptDataProxy(Sex);
            }
            if ((aProperty == "Occupation"))
            {
                return new Articy.Unity.Interfaces.ScriptDataProxy(Occupation);
            }
            if ((aProperty == "Accent"))
            {
                return new Articy.Unity.Interfaces.ScriptDataProxy(Accent);
            }
            if ((aProperty == "Personality"))
            {
                return new Articy.Unity.Interfaces.ScriptDataProxy(Personality);
            }
            if ((aProperty == "Appearance"))
            {
                return new Articy.Unity.Interfaces.ScriptDataProxy(Appearance);
            }
            return null;
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TocaBoca.Platform.Data;

namespace BandRacer.Plugin
{
    public class PlatformBase : NotifyPropertyChangedBase
    {
        public const string HasGameStartedPropertyName = "HasGameStarted";
        public const string HasIntroFinishedPropertyName = "HasIntroFinished";
        public const string IsSoundEnabledPropertyName = "IsSoundEnabled";
        public const string XAnglePlayer1PropertyName = "XAnglePlayer1";
        public const string YAnglePlayer1PropertyName = "YAnglePlayer1";

        public const string XAnglePlayer2PropertyName = "XAnglePlayer2";
        public const string YAnglePlayer2PropertyName = "YAnglePlayer2";


        private bool m_HasGameStarted;
        private bool m_HasIntroFinished;
        private float m_XAnglePlayer1;
        private float m_YAnglePlayer1;
        private float m_XAnglePlayer2;
        private float m_YAnglePlayer2;

        private readonly object m_LockObject;

        public static PlatformBase Current { get; set; }

        protected PlatformBase()
        {
            m_LockObject = new object();
        }

        public virtual float XAnglePlayer1
        {
            get { return m_XAnglePlayer1; }
            set
            {
                if (m_XAnglePlayer1 == value)
                    return;

                m_XAnglePlayer1 = value;
                OnPropertyChanged(XAnglePlayer1PropertyName);
            }
        }

        public virtual float XAnglePlayer2
        {
            get { return m_XAnglePlayer2; }
            set
            {
                if (m_XAnglePlayer2 == value)
                    return;

                m_XAnglePlayer2 = value;
                OnPropertyChanged(XAnglePlayer2PropertyName);
            }
        }

        public float YAnglePlayer1
        {
            get { return m_YAnglePlayer1; }
            set
            {
                if (m_YAnglePlayer1 == value)
                    return;

                m_YAnglePlayer1 = value;

                OnPropertyChanged(YAnglePlayer1PropertyName);
            }
        }

        public float YAnglePlayer2
        {
            get
            {
                return m_YAnglePlayer2;
            }
            set
            {
                if (m_YAnglePlayer2 == value)
                    return;

                m_YAnglePlayer2 = value;

                OnPropertyChanged(YAnglePlayer2PropertyName);
            }
        }

        public string Player1Name { get; set; }
        public string Player2Name { get; set; }

        public bool HasGameStarted
        {
            get { return m_HasGameStarted; }
            set
            {
                lock (m_LockObject)
                {
                    if (m_HasGameStarted)
                    {
                        return;
                    }

                    m_HasGameStarted = value;
                }

                OnPropertyChanged(HasGameStartedPropertyName);
            }
        }

        public bool HasIntroFinished
        {
            get { return m_HasIntroFinished; }
            set
            {
                lock (m_LockObject)
                {
                    if (m_HasIntroFinished)
                    {
                        return;
                    }

                    m_HasIntroFinished = value;
                }

                OnPropertyChanged(HasIntroFinishedPropertyName);
            }
        }

    }
}

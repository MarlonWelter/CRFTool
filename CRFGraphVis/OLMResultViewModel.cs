using CRFBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;

namespace CRFGraphVis
{
    public class OLMResultViewModel : INotifyPropertyChanged
    {
        private List<OLMEvaluationResult> evalResults;

        public List<OLMEvaluationResult> EvalResults
        {
            get { return evalResults ?? (evalResults = new List<OLMEvaluationResult>()); }
            set { evalResults = value; }
        }

        private int evalResPointer;

        public int EvalResPointer
        {
            get { return evalResPointer; }
            set
            {
                evalResPointer = Math.Max(0, Math.Min(value, EvalResults.Count-1));
                NotifyPropertyChanged("ViewModel");
            }
        }


        //private OLMEvaluationResult evalResult;

        public OLMEvaluationResult EvalResult
        {
            get { return EvalResults[EvalResPointer] ?? (new OLMEvaluationResult()); }
            //set
            //{
            //    evalResult = value;
            //    NotifyPropertyChanged("ViewModel");
            //}
        }

        private int graphPointer;

        public int GraphPointer
        {
            get { return graphPointer; }
            set
            {
                graphPointer = value % EvalResult.GraphResults.Count;
                NotifyPropertyChanged("ViewModel");
                NotifyPropertyChanged("GraphInFocus");
            }
        }

        private ViewType viewType;

        public ViewType ViewType
        {
            get { return viewType; }
            set
            {
                viewType = value;
                NotifyPropertyChanged("ViewModel");
            }
        }

        public OLMEvaluationGraphResult GraphInFocus => EvalResult.GraphResults[graphPointer];

        public OLMResultViewModel ViewModel => this;

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion
    }

    public enum ViewType
    {
        Default,
        Reference,
        Observation,
        Prediction
    }
}

﻿using NationalInstruments.RFmx.InstrMX;
using NationalInstruments.RFmx.SpecAnMX;
using NationalInstruments.ModularInstruments.NIRfsg;
using NationalInstruments.ModularInstruments.NIRfsgPlayback;
using static NationalInstruments.ReferenceDesignLibraries.SG;
using System;

namespace NationalInstruments.ReferenceDesignLibraries.SA
{
    public static class DPD
    {
        #region Type_Definitionss
        public struct CommonDPDConfiguration
        {
            public double dpdMeasurementInterval;
            public RFmxSpecAnMXDpdMeasurementSampleRateMode dpdSampleRateMode;
            public double sampleRate;
            public RFmxSpecAnMXDpdSignalType signalType;
            public RFmxSpecAnMXDpdReferenceWaveformIdleDurationPresent idleDurationPresent;   //TBD pull idle flag from SG
            public RFmxSpecAnMXDpdApplyDpdIdleDurationPresent dpdIdleDurationPresent;        //TBD pull idle flag from SG
            public RFmxSpecAnMXDpdPreDpdCfrEnabled preDpdCFREnabled;
            public RFmxSpecAnMXDpdApplyDpdCfrEnabled applyDpdCFREnabled;
            public double dutAverageInputPower;
            public double prefilterGain;           
            public double rfsgIqRate;
            public double timeout;


            public static CommonDPDConfiguration GetDefault()
            {
                return new CommonDPDConfiguration
                {
                    timeout = 3,
                    dpdMeasurementInterval = 100e-6,
                    prefilterGain = -1.5,
                    dpdSampleRateMode = RFmxSpecAnMXDpdMeasurementSampleRateMode.ReferenceWaveform,
                    sampleRate = 320e6,
                    signalType = RFmxSpecAnMXDpdSignalType.Modulated,
                    dutAverageInputPower = -20,
                    preDpdCFREnabled = RFmxSpecAnMXDpdPreDpdCfrEnabled.False,
                    applyDpdCFREnabled = RFmxSpecAnMXDpdApplyDpdCfrEnabled.False
                };
            }

        }
        public struct LUTDPDConfiguration
        {
            public RFmxSpecAnMXDpdLookupTableThresholdType thresholdType;
            public RFmxSpecAnMXDpdLookupTableThresholdEnabled thresholdEnabled; 
            public double thresholdLevel;
            public double LUTStepSize;
            public RFmxSpecAnMXDpdApplyDpdLookupTableCorrectionType LUTCorrectionType;
            public RFmxSpecAnMXDpdLookupTableType lutType;

            public static LUTDPDConfiguration GetDefault()
            {
                return new LUTDPDConfiguration
                {
                    lutType = RFmxSpecAnMXDpdLookupTableType.Linear,
                    thresholdType = RFmxSpecAnMXDpdLookupTableThresholdType.Relative,
                    thresholdEnabled = RFmxSpecAnMXDpdLookupTableThresholdEnabled.True,
                    thresholdLevel = -20,
                    LUTStepSize = 0.1,
                    LUTCorrectionType = RFmxSpecAnMXDpdApplyDpdLookupTableCorrectionType.MagnitudeAndPhase
          
                };
            }
        }

        public struct MPDPDConfiguration
        {
            public RFmxSpecAnMXDpdApplyDpdMemoryModelCorrectionType memoryModelCorrectionType;
            public RFmxSpecAnMXDpdIterativeDpdEnabled iterativeDpdEnabled;
            public int numberOfIteration;
            public int memoryPolynomialOrder, memoryPolynomialDepth, memoryPolynomialLeadOrder, memoryPolynomialLagOrder,
            memoryPolynomialLeadMemoryDepth, memoryPolynomialLagMemoryDepth, memoryPolynomialMaxLead, memoryPolynomialMaxLag;
            public static MPDPDConfiguration GetDefault()
            {
                return new MPDPDConfiguration
                {
                    memoryModelCorrectionType = RFmxSpecAnMXDpdApplyDpdMemoryModelCorrectionType.MagnitudeAndPhase,
                    iterativeDpdEnabled = RFmxSpecAnMXDpdIterativeDpdEnabled.False,
                    numberOfIteration = 3,
                    memoryPolynomialDepth = 2,
                    memoryPolynomialOrder = 3,
                    memoryPolynomialLeadOrder = 2,
                    memoryPolynomialLagOrder = 2,
                    memoryPolynomialLeadMemoryDepth = 2,
                    memoryPolynomialLagMemoryDepth = 2,
                    memoryPolynomialMaxLead = 2,
                    memoryPolynomialMaxLag = 2
            };
            }
        }

        public struct LUTResults
        {
            public ComplexWaveform<ComplexSingle> waveformWithDpdComplexSingle;
            public float[] lookUpTableInputPowers;
            public ComplexSingle[] lookUpTableComplexGains;
            public double postDPDPAPR;  
            public double powerOffset;   
        }

        public struct MPResults
        {
            public ComplexWaveform<ComplexSingle> waveformWithDpdComplexSingle;
            public ComplexSingle[] dpdPolynomial;
            public double postDPDPAPR;  
            public double powerOffset;   
        }
        #endregion
        #region ConfigureDPD
        public static void ConfigureCommonDPD(RFmxSpecAnMX specAnSignal, CommonDPDConfiguration commonDPDConfig, Waveform waveform)
        {
            specAnSignal.SelectMeasurements("", RFmxSpecAnMXMeasurementTypes.Dpd, true);
            
            specAnSignal.Dpd.Configuration.ConfigureReferenceWaveform("",waveform.WaveformData,commonDPDConfig.idleDurationPresent,
                 commonDPDConfig.signalType);
            specAnSignal.Dpd.Configuration.ConfigureDutAverageInputPower("", commonDPDConfig.dutAverageInputPower);
            specAnSignal.Dpd.Configuration.ConfigureMeasurementInterval("", commonDPDConfig.dpdMeasurementInterval);
            specAnSignal.Dpd.Configuration.ConfigureMeasurementSampleRate("", commonDPDConfig.dpdSampleRateMode, commonDPDConfig.sampleRate);

        }
       
        public static void ConfigureRFmxLUT(RFmxSpecAnMX specAnSignal, LUTDPDConfiguration lutConfig)
        {
            specAnSignal.Dpd.Configuration.ConfigureDpdModel("", RFmxSpecAnMXDpdModel.LookupTable);
            specAnSignal.Dpd.Configuration.ConfigureLookupTableThreshold("", lutConfig.thresholdEnabled, lutConfig.thresholdLevel,
                lutConfig.thresholdType);
            specAnSignal.Dpd.Configuration.ConfigureLookupTableStepSize("", lutConfig.LUTStepSize);
        }
      
        public static void ConfigureRFmxMP(RFmxSpecAnMX specAnSignal, MPDPDConfiguration mpConfig)
        {
            specAnSignal.Dpd.Configuration.ConfigureDpdModel("", RFmxSpecAnMXDpdModel.GeneralizedMemoryPolynomial);
            specAnSignal.Dpd.Configuration.ConfigureMemoryPolynomial("", mpConfig.memoryPolynomialOrder, mpConfig.memoryPolynomialDepth);
            specAnSignal.Dpd.Configuration.ConfigureGeneralizedMemoryPolynomialCrossTerms("", mpConfig.memoryPolynomialLeadOrder,
                mpConfig.memoryPolynomialLagOrder, mpConfig.memoryPolynomialLeadMemoryDepth, mpConfig.memoryPolynomialLagMemoryDepth,
                mpConfig.memoryPolynomialMaxLead, mpConfig.memoryPolynomialMaxLag);
            specAnSignal.Dpd.Configuration.ConfigureIterativeDpdEnabled("", mpConfig.iterativeDpdEnabled);
            specAnSignal.Dpd.ApplyDpd.ConfigureMemoryModelCorrectionType("", mpConfig.memoryModelCorrectionType);
        }
        #endregion

        #region ApplyDPD
        public static LUTResults ApplyLUTDPD(RFmxSpecAnMX specAnSignal, NIRfsg rfsgSession, CommonDPDConfiguration commonDPDConfig, LUTDPDConfiguration lutConfig, Waveform waveform, string waveformStartTriggerExport, string selectorString = "")
        {
            LUTResults lutResults = new LUTResults();
            IntPtr rfsgPtr;
            rfsgPtr = rfsgSession.GetInstrumentHandle().DangerousGetHandle();
            specAnSignal.Initiate("", selectorString);
            specAnSignal.Dpd.ApplyDpd.ConfigureLookupTableCorrectionType("", lutConfig.LUTCorrectionType);
            specAnSignal.Dpd.ApplyDpd.ApplyDigitalPredistortion(selectorString, waveform.WaveformData,commonDPDConfig.dpdIdleDurationPresent , commonDPDConfig.timeout, ref lutResults.waveformWithDpdComplexSingle,
                out lutResults.postDPDPAPR, out lutResults.powerOffset);

            waveform.WaveformData = lutResults.waveformWithDpdComplexSingle;
            waveform.SampleRate = 1 / lutResults.waveformWithDpdComplexSingle.PrecisionTiming.SampleInterval.TotalSeconds;
            waveform.PAPR_dB = lutResults.postDPDPAPR;
            waveform.SignalBandwidth_Hz = 0.8 * waveform.SampleRate;

            rfsgSession.Abort();
            SG.DownloadWaveform(rfsgSession, waveform);
            rfsgSession.RF.PowerLevel = commonDPDConfig.dutAverageInputPower - lutResults.powerOffset;
            rfsgSession.Initiate();
            specAnSignal.Dpd.Results.FetchLookupTable(selectorString, commonDPDConfig.timeout, ref lutResults.lookUpTableInputPowers, ref lutResults.lookUpTableComplexGains);
            return lutResults;

        }
      
        public static MPResults ApplyMPDPD(RFmxSpecAnMX specAnSignal, NIRfsg rfsgSession, CommonDPDConfiguration commonDPDConfig,MPDPDConfiguration mpConfig, Waveform waveform, string waveformStartTriggerExport, string selectorString = "")
        {
            MPResults mPResults = new MPResults();
            IntPtr rfsgPtr;
            rfsgPtr = rfsgSession.GetInstrumentHandle().DangerousGetHandle();
            if (mpConfig.iterativeDpdEnabled == RFmxSpecAnMXDpdIterativeDpdEnabled.False)
            {
                mpConfig.numberOfIteration = 1;
            }

            for (int i = 0; i < mpConfig.numberOfIteration; i++)
            {
                specAnSignal.Dpd.Configuration.ConfigurePreviousDpdPolynomial("", mPResults.dpdPolynomial);
                specAnSignal.Initiate("", selectorString);
                specAnSignal.Dpd.ApplyDpd.ApplyDigitalPredistortion(selectorString, waveform.WaveformData, commonDPDConfig.dpdIdleDurationPresent, commonDPDConfig.timeout, ref mPResults.waveformWithDpdComplexSingle,
               out mPResults.postDPDPAPR, out mPResults.powerOffset);

                waveform.WaveformData = mPResults.waveformWithDpdComplexSingle;
                waveform.SampleRate = 1 / mPResults.waveformWithDpdComplexSingle.PrecisionTiming.SampleInterval.TotalSeconds;
                waveform.PAPR_dB = mPResults.postDPDPAPR ;
                waveform.SignalBandwidth_Hz = 0.8 * waveform.SampleRate;             
                
                rfsgSession.Abort();
                SG.DownloadWaveform(rfsgSession, waveform);
                rfsgSession.RF.PowerLevel = commonDPDConfig.dutAverageInputPower - mPResults.powerOffset;
                rfsgSession.Initiate();
                specAnSignal.Dpd.Results.FetchDpdPolynomial(selectorString, 3, ref mPResults.dpdPolynomial);
            }
            return mPResults;


        }
        #endregion


    }
}
   

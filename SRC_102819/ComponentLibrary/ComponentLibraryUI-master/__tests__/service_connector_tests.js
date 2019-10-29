import React from 'react';
import chai, {expect} from 'chai';
import sinon from 'sinon';
import sinonChai from 'sinon-chai';
import {apiUrl} from '../src/helpers'
import {mapStateToProps} from '../src/service_connector';
import {mapDispatchToProps,__RewireAPI__ as ConnectorRewired} from '../src/service_connector';
import moment from 'moment';

chai.use(sinonChai);

describe('ServiceConnector', () => {
  describe('MapStateToProps', () => {
    it('should return current service details, service code and error as empty string', () => {
      const props = {params: {serviceCode: 'SR0001'}};
      const state = {reducer:
          {
            components:{
            'SR0001':{
              serviceDetails:{
                    serviceName: 'service'
                  }
              }
            },
            error: "",
            serviceRates:{Id: 'FDP0001'},
            rateserror:"some error"
          }
      };
      const actualOutput={
        serviceCode: 'SR0001',
        details: {serviceName: 'service'},
        classificationDefinition:undefined,
        error: "",
        rates:{Id: 'FDP0001'},
        rateserror:'some error'
      };
      const expectedOutput = mapStateToProps(state, props);
      expect(expectedOutput).to.deep.equal(actualOutput);
    });
  });
  describe('MapDispatchToProps', () => {
    let dispatchSpy, dispatchToProps, fetchStub;
    beforeEach(()=>{
      dispatchSpy = sinon.spy();
      dispatchToProps = mapDispatchToProps(dispatchSpy);
    });

    describe('ServiceFetchRequest', () => {
      it('should return SERVICE_FETCH_SUCCEEDED when provided with valid Service', async () => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.resolve({data: {headers:[{key: 'serviceDetails'}]}}))});
        await dispatchToProps.onServiceFetchRequest('SR001');
        expect(dispatchSpy).to.be.calledWith({
            type: 'SERVICE_FETCH_SUCCEEDED',
            details: { classificationDefinition: undefined, serviceDetails: {headers: [{key: 'serviceDetails'}] }}
          });
      });

      it('should return SERVICE_FETCH_FAILED when response status is 500', async ()=>{
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub()
          .returns(Promise.reject({response:{status: 500}}))});
        await dispatchToProps.onServiceFetchRequest('SR_Invalid');
        expect(dispatchSpy).to.be.calledWith({
          type:'SERVICE_FETCH_FAILED',
          serviceCode:'SR_Invalid',
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      });

      it('should return SERVICE_FETCH_FAILED when response status is 403', async ()=>{
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub()
          .returns(Promise.reject({response:{status: 403}}))});
        await dispatchToProps.onServiceFetchRequest('SR_Invalid');
        expect(dispatchSpy).to.be.calledWith({
          type:'SERVICE_FETCH_FAILED',
          serviceCode:'SR_Invalid',
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      });

      it('should return SERVICE_FETCH_FAILED when response status is 404', async ()=>{
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub()
          .returns(Promise.reject({response:{status: 404}}))});
        await dispatchToProps.onServiceFetchRequest('SR_Invalid');
        expect(dispatchSpy).to.be.calledWith({
          type:'SERVICE_FETCH_FAILED',
          serviceCode:'SR_Invalid',
          error: 'No Service with code: SR_Invalid is found.'
        });
      });
      it('should return SERVICE_FETCH_FAILED when an error occurs', async ()=>{
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub()
          .returns(Promise.reject({message:'Material not found.'}))});
        await dispatchToProps.onServiceFetchRequest('SR_Invalid');
        expect(dispatchSpy).to.be.calledWith({
          type:'SERVICE_FETCH_FAILED',
          serviceCode:'SR_Invalid',
          error: 'Material not found.'
        });
      });
    });

    describe('OnServiceRatesFetchRequest', () => {

      it('should dispatch SERVICE_RATES_FETCH_SUCCEEDED when service rates exists', async () => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.resolve({data:'serviceRates'}))});
        const now = moment.utc().format();
        await dispatchToProps.onServiceRatesFetchRequest('FDP0001');
        expect(dispatchSpy).to.be.calledWith({
          type: 'SERVICE_RATES_FETCH_SUCCEEDED',
          rates: 'serviceRates'});
      });

      it('should dispatch SERVICE_RATES_FETCH_FAILED when fetch throws an error', async () => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub()
          .returns(Promise.reject({response:{data:{message:'Service rates are not found'}}}))});
        await dispatchToProps.onServiceRatesFetchRequest('FDP0001');

        expect(dispatchSpy).to.be.calledWith({
          type: 'SERVICE_RATES_FETCH_FAILED',
          error: 'Service rates are not found'
        });
      });

      it('should dispatch SERVICE_RATES_FETCH_FAILED when response status is 500',async () => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub()
          .returns(Promise.reject({response:{status:500}}))});
        await dispatchToProps.onServiceRatesFetchRequest('FDP0001');
        expect(dispatchSpy).to.be.calledWith({
          type: 'SERVICE_RATES_FETCH_FAILED',
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      });

      it('should dispatch SERVICE_RATES_FETCH_FAILED when response status is 404',async () => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub()
          .returns(Promise.reject({response:{status:404}}))});
        await dispatchToProps.onServiceRatesFetchRequest('FDP0001');
        expect(dispatchSpy).to.be.calledWith({
          type: 'SERVICE_RATES_FETCH_FAILED',
          error: 'No service Rates with code: FDP0001 is found'
        });
      });
    });
  });
});

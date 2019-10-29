import sinon from 'sinon';
import {mapStateToProps, mapDispatchToProps, __RewireAPI__ as ConnectorRewired} from '../src/material_connector';
import chai, {expect} from 'chai';
import {apiUrl} from '../src/helpers';
import sinonChai from 'sinon-chai';
import moment from 'moment'

// import 'babel-polyfill';

chai.use(sinonChai);

describe('Material Connector', () => {
  describe('mapStateToProps', () => {
    it('should return current material details and material rates with materialCode as parameter', () => {
      const state = {
        reducer: {
          components: {'MT0001': {name: "abc"}},
          currentMaterialRates: [{id: "IRN000002", location: "Hyderabad", typeOfPurchase: "DOMESTIC INTER-STATE",}],
          newMaterialError: '',
          rateserror: '',
          currentRentalRates: 'Rental Rates',
          rentalRatesError: 'Rental Rates Error'
        }
      };
      const props = {params: {materialCode: 'MT0001'}};
      const actual = mapStateToProps(state, props);
      const expected = {
        materialCode: 'MT0001',
        details: {name: "abc"},
        rates: [{id: "IRN000002", location: "Hyderabad", typeOfPurchase: "DOMESTIC INTER-STATE",}],
        detailserror: '',
        rateserror: '',
        rentalRates: 'Rental Rates',
        rentalRatesError: 'Rental Rates Error'
      };
      expect(actual).to.deep.equal(expected);
    });
  });

  describe('mapDispatchToProps', () => {
    describe('onMaterialFetchRequest', () => {
      let dispatchSpy, returnedObject, fetchStub;
      beforeEach(() => {
        dispatchSpy = sinon.spy();
        returnedObject = mapDispatchToProps(dispatchSpy);
      });

      it('should dispatch MATERIAL_FETCH_SUCCEEDED when material exists', async() => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.resolve({data: 'materialDetails'}))});
        await returnedObject.onMaterialFetchRequest('MT0001');
        expect(dispatchSpy).to.be.calledWith({
          type: 'MATERIAL_FETCH_SUCCEEDED',
          details: 'materialDetails'
        });
      });
      it('should dispatch MATERIAL_FETCH_FAILED when fetch throws an error', async() => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.reject({message: 'Material not found'}))});

        await returnedObject.onMaterialFetchRequest('MT0001');

        expect(dispatchSpy).to.be.calledWith({
          type: 'MATERIAL_FETCH_FAILED',
          materialCode: 'MT0001',
          detailserror: 'Material not found'
        });
      });

      it('should dispatch MATERIAL_FETCH_FAILED when response status is 500', async() => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub()
          .returns(Promise.reject({response:{status: 500}}))});
        await returnedObject.onMaterialFetchRequest('SFT00003');
        expect(dispatchSpy).to.be.calledWith({
          type: 'MATERIAL_FETCH_FAILED',
          materialCode: 'SFT00003',
          detailserror: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      });

      it('should dispatch MATERIAL_FETCH_FAILED when response status is 404', async() => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub()
          .returns(Promise.reject({response:{status: 404}}))});
        await returnedObject.onMaterialFetchRequest('SFT00003');
        expect(dispatchSpy).to.be.calledWith({
          type: 'MATERIAL_FETCH_FAILED',
          materialCode: 'SFT00003',
          detailserror: 'No Material with code: SFT00003 is found.'
        });
      });
    });

    describe('onMaterialRatesFetchRequest', () => {
      let dispatchSpy, returnedObject, fetchStub;
      beforeEach(() => {
        dispatchSpy = sinon.spy();
        returnedObject = mapDispatchToProps(dispatchSpy);
      });

      it('should dispatch MATERIAL_RATES_FETCH_SUCCEEDED when material rates exists', async() => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.resolve({data: 'materialRates'}))});
        const now = moment.utc().format();
        await returnedObject.onMaterialRatesFetchRequest('MT0001');
        expect(dispatchSpy).to.be.calledWith({
          type: 'MATERIAL_RATES_FETCH_SUCCEEDED',
          rates: 'materialRates'
        });
      });

      it('should dispatch MATERIAL_RATES_FETCH_FAILED when fetch throws an error', async() => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.reject({response:{data:{message: 'Material rates not found'}}}))});
        await returnedObject.onMaterialRatesFetchRequest('MT0001');

        expect(dispatchSpy).to.be.calledWith({
          type: 'MATERIAL_RATES_FETCH_FAILED',
          rateserror: 'Material rates not found'
        });
      });

      it('should dispatch MATERIAL_RATES_FETCH_FAILED when response status is 500', async() => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub()
          .returns(Promise.reject({response:{status: 500}}))});
        await returnedObject.onMaterialRatesFetchRequest('SFT00003');
        expect(dispatchSpy).to.be.calledWith({
          type: 'MATERIAL_RATES_FETCH_FAILED',
          rateserror: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      });

      it('should dispatch MATERIAL_RATES_FETCH_FAILED when response status is 404', async() => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub()
          .returns(Promise.reject({response:{status: 404}}))});
        await returnedObject.onMaterialRatesFetchRequest('SFT00003');
        expect(dispatchSpy).to.be.calledWith({
          type: 'MATERIAL_RATES_FETCH_FAILED',
          rateserror: 'Material Rates are not found'
        });
      });
    });

    describe('onRentalRatesFetchRequest', () => {
        let dispatchSpy, returnedObject;
        beforeEach(() => {
        dispatchSpy = sinon.spy();
        returnedObject = mapDispatchToProps(dispatchSpy);
      });

      it('should dispatch RENTAL_RATES_FETCH_SUCCEEDED when material rates exists', async() => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub()
          .returns(Promise.resolve({data: 'rentalRates'}))});
          await returnedObject.onRentalRatesFetchRequest('MT0001');
        expect(dispatchSpy).to.be.calledWith({
          type: 'RENTAL_RATES_FETCH_SUCCEEDED',
          rentalRates: 'rentalRates'
        });
      });

      it('should dispatch RENTAL_RATES_FETCH_FAILED when fetch throws an error', async() => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub()
          .returns(Promise.reject({message: 'Rental rates not found'}))});
        await returnedObject.onRentalRatesFetchRequest('MT0001');
        expect(dispatchSpy).to.be.calledWith({
          type: 'RENTAL_RATES_FETCH_FAILED',
          rentalRatesError: 'Rental rates not found',
        });
      });

      it('should dispatch RENTAL_RATES_FETCH_FAILED when response status is 500', async() => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub()
          .returns(Promise.reject({response:{status: 500}}))});
        await returnedObject.onRentalRatesFetchRequest('SFT00003');
        expect(dispatchSpy).to.be.calledWith({
          type: 'RENTAL_RATES_FETCH_FAILED',
          rentalRatesError: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      });

      it('should dispatch RENTAL_RATES_FETCH_FAILED when response status is 404', async() => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub()
          .returns(Promise.reject({response:{status: 404}}))});
        await returnedObject.onRentalRatesFetchRequest('SFT00003');
        expect(dispatchSpy).to.be.calledWith({
          type: 'RENTAL_RATES_FETCH_FAILED',
          rentalRatesError: 'Rental Rates are not found'
        });
      });
    });

    describe('onMaterialDestroy', ()=>{
      let dispatchSpy, returnedObject;
      beforeEach(() => {
          dispatchSpy = sinon.spy();
          returnedObject = mapDispatchToProps(dispatchSpy);
      });
      it('should call dispatch with MATERIAL_DESTROYED', ()=>{
        returnedObject.onMaterialDestroy();
        expect(dispatchSpy).to.be.calledWith({type:'MATERIAL_DESTROYED'});
      })
    });
  });
});

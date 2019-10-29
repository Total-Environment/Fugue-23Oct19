import React from 'react';
import chai, {expect} from 'chai';
import {shallow} from 'enzyme';
import chaiEnzyme from 'chai-enzyme';
import sinonChai from 'sinon-chai';
import sinon from 'sinon';
import {RatesHome} from '../../src/components/rates-home/index';


chai.use(chaiEnzyme());
chai.use(sinonChai);


describe('RatesHome', () => {
  let onRatesFetchRequestSpy, onDependencyDefinitionFetchSpy, fetchMasterDataByNameIfNeededSpy, props, wrapper;
  beforeEach(() => {
    onRatesFetchRequestSpy = sinon.spy();
    onDependencyDefinitionFetchSpy = sinon.spy();
    fetchMasterDataByNameIfNeededSpy = sinon.spy();
    props = {
      onRatesFetchRequest: onRatesFetchRequestSpy,
      onDependencyDefinitionFetch: onDependencyDefinitionFetchSpy,
      fetchMasterDataByNameIfNeeded: fetchMasterDataByNameIfNeededSpy,
      masterData: [],
      classificationData: {values: {'values': 'values'}, isFetching: false},
      componentType: 'material',
      filters: [{columnKey: 'Location', columnValue: 'Benguluru'}],
    };
  });
  describe('ComponentDidMount', () => {
    it('should call onRatesFetchRequest when it does not have rates', () => {
      (new RatesHome(props)).componentWillMount();
      expect(onRatesFetchRequestSpy).to.be.calledWith(props.filters);
    });

    it('should call onDependencyDefinitionFetch when it does not have rates', () => {
      props.classificationData = undefined;
      (new RatesHome(props)).componentWillMount();
      expect(onDependencyDefinitionFetchSpy).to.be.calledWith('materialClassifications');
    });
  });

  describe('render', () => {
    beforeEach(() => {
      props.rates = [
          {
            "materialRate": {
              "controlBaseRate": {
                "value": 89,
                "currency": "INR"
              },
              "landedRate": {
                "value": 10359.54,
                "currency": "INR"
              },
              "procurementRateThreshold": null,
              "insuranceCharges": 87,
              "freightCharges": 89,
              "basicCustomsDuty": 876,
              "clearanceCharges" : 324,
              "taxVariance" : 8,
              "locationVariance" : 897,
              "marketFluctuation": 8978,
              "typeOfPurchase": "IMPORT",
              "location": "Bang",
              "id": "ALM000001",
              "appliedOn": "2017-05-11T18:30:00Z"
            },
            "materialName": "Aluminium angle"
          }
        ];
      props.ratesError = {material:'', service :''};
      props.masterData = {'status':{id: 'id', name: 'status', values:{ values: ["approved,inactive"]}},
        'type_of_purchase':{id: 'id', name: 'type_of_purchase', values:{ values: ["approved,inactive"]}},
        'location':{id: 'id', name: 'location', values:{ values: ["approved,inactive"]}}, currency: {id: 'id', name: 'currency', values:{ values: ["INR,USD"]}}};
      wrapper = shallow(<RatesHome {...props}/>);
    });

    it('should render ReactDataGrid', () => {
      expect(wrapper).to.have.descendants('ReactDataGrid');
    });

    it('should render loading if rates does not exist', () => {
      props.rates = undefined;
      wrapper = shallow(<RatesHome {...props}/>);
      expect(wrapper).to.have.descendants('Loading');
    });

    // it('should change state when grip rows get updated', () => {
    //   const expected = [{
    //     materialName: 'Aluminium Straight Edge',
    //     code: 'ALM000039',
    //     typeOfPurchase: 'Import',
    //     controlBaseRate: 123,
    //     currencyType: 'USD',
    //     appliedOn: {value:'2017-05-14T18:30:00Z',rowIndex:0},
    //     'Customs Duty': 12,
    //     'Location Variance': 45,
    //     'Insurance Considered by Customs':99,
    //     'Freight':67,
    //     'Clearing Charges by Customs': 478,
    //     'Market Fluctuation': "23",
    //     'Tax Variance': "738",
    //   }];
    //   wrapper.find('ReactDataGrid').simulate('gridRowsUpdated',{fromRow:0,toRow:0,updated: {'controlBaseRate': 123}});
    //   expect(wrapper).to.have.state('rows').deep.equal(expected);
    // });
  });

});

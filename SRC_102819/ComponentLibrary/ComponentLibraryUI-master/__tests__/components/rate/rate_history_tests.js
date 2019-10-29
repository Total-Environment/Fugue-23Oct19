import React from 'react';
import chai, {expect} from 'chai';
import {shallow} from 'enzyme';
import chaiEnzyme from 'chai-enzyme';
import sinonChai from 'sinon-chai';
import sinon from 'sinon';
import {RateHistory} from '../../../src/components/rate-history/index'

chai.use(chaiEnzyme());
chai.use(sinonChai);

describe('RateHistory', () => {
  describe('ComponentDidMount', () => {
    let onRateHistoryFetchRequestSpy, props, wrapper;
    beforeEach(() => {
      onRateHistoryFetchRequestSpy = sinon.spy();
      props = {
        componentCode: 'MT0001',
        onRateHistoryFetchRequest: onRateHistoryFetchRequestSpy,
        error: '',
        componentType: 'material',
        fetchMasterDataByNameIfNeeded: sinon.spy(),
      };
      wrapper = shallow(<RateHistory {...props}/>);
    });

    it('should call fetch data when it does not have rate history', () => {
      wrapper.instance().componentDidMount();
      expect(onRateHistoryFetchRequestSpy).to.be.calledWith('MT0001', 'material');
      expect(props.fetchMasterDataByNameIfNeeded).to.have.been.calledWith('location');
      expect(props.fetchMasterDataByNameIfNeeded).to.have.been.calledWith('type_of_purchase');
    });

  });
  describe('render', () => {
    let props, wrapper;
    beforeEach(() => {
      props = {
        rateHistory: {
          items: [{
            "insuranceCharges" : 2,
            "freightCharges" : 0,
            "basicCustomsDuty" : 3,
            "clearanceCharges" : 5,
            "taxVariance" : 8,
            "loationVariance" : 4,
            "marketFluctuation" : 0,
            "controlBaseRate": {
              "value": 60,
              "currency": "USD"
            },
            "landedRate": {
              "value": 117,
              "currency": "USD"
            },
            "typeOfPurchase": "IMPORT",
            "location": "Hyderabad",
            "id": "MNF000171",
            "appliedOn": "2016-12-13T00:00:00Z"
          }],
          pageNumber: 1,
          sortColumn: 'Material Name',
          sortOrder: 'Ascending',
        },
        error: '',
        componentCode: 'MNF000171',
        componentType: 'material'
      };
      wrapper = shallow(<RateHistory {...props}/>);
    });

    it('should render Table', () => {
      const table = wrapper.find('Table');
      const expectedHeaders = [{
        "name": "Location",
        "key": "location",
        "sortKey": "Location",
        "type": "text",
        "sortable": true
      }, {
        "name": "Type of Purchase",
        "key": "typeOfPurchase",
        "sortKey": "TypeOfPurchase",
        "type": "text",
        "sortable": true
      }, {
        "name": "Landed Rate",
        "key": "landedRate",
        "sortKey": "LandedRate.Value",
        "type": "number",
        "sortable": false
      },{
        "name": "Control Base Rate",
        "key": "controlBaseRate",
        "sortKey": "ControlBaseRate.Value",
        "type": "number",
        "sortable": false
      },{
        "name": "Insurance Charges (%)",
        "key": "insuranceCharges",
        "sortKey": "insuranceCharges",
        type: 'number',
        "sortable": false
      }, {
        "name": "Freight Charges (%)",
        "key": "freightCharges",
        "sortKey": "freightCharges",
        type: 'number',
        "sortable": false
      }, {
        "name": "Basic Customs Duty (%)",
        "key": "basicCustomsDuty",
        "sortKey": "basicCustomsDuty",
        type: 'number',
        "sortable": false
      }, {
        "name": "Clearance Charges (%)",
        "key": "clearanceCharges",
        "sortKey": "clearanceCharges",
        type: 'number',
        "sortable": false
      }, {
        "name": "Tax Variance (%)",
        "key": "taxVariance",
        "sortKey": "taxVariance",
        type: 'number',
        "sortable": false
      }, {
        "name": "Location Variance (%)",
        "key": "locationVariance",
        "sortKey": "locationVariance",
        type: 'number',
        "sortable": false
      },
      {
        "name": "Market Fluctuation (%)",
        "key": "marketFluctuation",
        "sortKey": "marketFluctuation",
        type: 'number',
        "sortable": false
      },
      {"name": "Applied From", "key": "appliedOn", "sortKey": "AppliedOn", "type": "number", "sortable": true}];
      console.log(JSON.stringify(table.prop('headers')));
      expect(table).to.have.prop('headers').deep.equal(expectedHeaders);
      expect(table).to.have.prop('data');
    });
  });
});

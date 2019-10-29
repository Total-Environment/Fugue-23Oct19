import React from 'react';
import {shallow} from 'enzyme';
import {CreateRateActual, __RewireAPI__ as CreateRateRewired} from '../../src/components/create-rate';
import chai, {expect} from 'chai';
import chaiEnzyme from 'chai-enzyme';
import sinonChai from 'sinon-chai';
import sinon from 'sinon';
import {idFor} from '../../src/helpers';

chai.use(chaiEnzyme());
chai.use(sinonChai);

describe('CreateRate', () => {
    let props, onAddRateErrorClose, onAddRate, wrapper;
    const location = "location";
    const typeOfPurchase = "Domestic";
    const controlBaseRate = 1000;
    const currency = "INR";
    const basicCustomsDuty = 100;
    const locationVariance = 100;
    const marketFluctuation = 10;
    const freightCharges = 100;
    const insuranceCharges = 100;
    const clearanceCharges = 100;
    const taxVariance = 10;
    const appliedOn = "03/04/2017";
    const expectedJson = {
        "location": location,
        "typeOfPurchase": typeOfPurchase,
        "appliedOn": appliedOn,
        "id": "CLY0001",
        "controlBaseRate": {
          "value": controlBaseRate,
          "currency": currency
        },
        'insuranceCharges' : insuranceCharges,
        'freightCharges' : freightCharges,
        'basicCustomsDuty' : basicCustomsDuty,
        'clearanceCharges' : clearanceCharges,
        'taxVariance' : taxVariance,
        'locationVariance' : locationVariance,
        'marketFluctuation' : marketFluctuation
    };
    beforeEach(() => {
        onAddRate = sinon.spy();
        onAddRateErrorClose = sinon.spy();
        props = {
            addRateError: "",
            componentType: 'material',
            onAddRateErrorClose: onAddRateErrorClose,
            onAddRate: onAddRate,
            componentCode: 'CLY0001',
            rateAdding: false,
            locationData: {isFetching: false, values: {values: []}},
            typeOfPurchaseData: {isFetching: false, values: {values: []}},
            currencyData: {isFetching: false, values: {values: []}},
        };
        wrapper = shallow(<CreateRateActual {...props}/>);
    });
    it('should render Collapse panel on click of Add New Button', () => {

        expect(wrapper).to.have.descendants('button');
        // wrapper.find('button').at(0).simulate('click');
        expect(wrapper).to.have.descendants('h2');
    });

    it('should render show error msg when location in empty', () => {
        wrapper.find('button').at(0).simulate('click');
      expect(wrapper.containsMatchingElement(<span>The field cannot be left blank.</span>)).to.equals(true);
    });

    it('should call add rate with valid input', () => {

        shallow(wrapper.find('CollapsePanel').prop('header')).find('#addNew').simulate('click');
        wrapper.find(`#${idFor("Control Base Rate")}`).first().simulate('change', controlBaseRate);
        wrapper.find(`#${idFor("Applied from")}`).first().simulate('change', appliedOn);
        wrapper.find('#location').first().simulate('change', {target: {value: location}});
        wrapper.find('#currency').first().simulate('change', {target: {value: currency}});
        wrapper.find('#typeOfPurchase').first().simulate('change', {target: {value: typeOfPurchase}});
        wrapper.find(`#${idFor('basicCustomsDuty')}`).first().simulate('change', basicCustomsDuty);
        wrapper.find(`#${idFor('locationVariance')}`).first().simulate('change',locationVariance);
        wrapper.find(`#${idFor('marketFluctuation')}`).first().simulate('change',  marketFluctuation);
        wrapper.find(`#${idFor('freightCharges')}`).first().simulate('change',  freightCharges);
        wrapper.find(`#${idFor('insuranceCharges')}`).first().simulate('change',  insuranceCharges);
        wrapper.find(`#${idFor('clearanceCharges')}`).first().simulate('change', clearanceCharges);
        wrapper.find(`#${idFor('taxVariance')}`).first().simulate('change', taxVariance);
        wrapper.find('#addForm').first().simulate('click');
        wrapper.find('#ConfirmationDialog').simulate('yes');
        expect(onAddRate).to.be.calledWith(expectedJson, 'material');
    });

    it('should show mandatory fields when control base rate is not filled',()=>{
        CreateRateRewired.__Rewire__('styles', {error: 'error'});
        shallow(wrapper.find('CollapsePanel').prop('header')).find('#addNew').simulate('click');
        // wrapper.find(`#${idFor("Control Base Rate")}`).first().simulate('change', controlBaseRate);
        wrapper.find(`#${idFor("Applied from")}`).first().simulate('change', appliedOn);
        wrapper.find('#location').first().simulate('change', {target: {value: location}});
        wrapper.find('#currency').first().simulate('change', {target: {value: currency}});
        wrapper.find('#typeOfPurchase').first().simulate('change', {target: {value: typeOfPurchase}});
        wrapper.find(`#${idFor('basicCustomsDuty')}`).first().simulate('change', basicCustomsDuty);
        wrapper.find(`#${idFor('locationVariance')}`).first().simulate('change',locationVariance);
        wrapper.find(`#${idFor('marketFluctuation')}`).first().simulate('change',  marketFluctuation);
        wrapper.find(`#${idFor('freightCharges')}`).first().simulate('change',  freightCharges);
        wrapper.find(`#${idFor('insuranceCharges')}`).first().simulate('change',  insuranceCharges);
        wrapper.find(`#${idFor('clearanceCharges')}`).first().simulate('change', clearanceCharges);
        wrapper.find(`#${idFor('taxVariance')}`).first().simulate('change', taxVariance);
        wrapper.find('#addForm').first().simulate('click');
        expect(wrapper.find(`#${idFor("Control Base Rate")}`).first()).to.have.className('error');
    });

    it('should show mandatory fields when Applied from is not filled',()=>{
        CreateRateRewired.__Rewire__('styles', {error: 'error'});
        shallow(wrapper.find('CollapsePanel').prop('header')).find('#addNew').simulate('click');
        wrapper.find(`#${idFor("Control Base Rate")}`).first().simulate('change', controlBaseRate);
        // wrapper.find(`#${idFor("Applied from")}`).first().simulate('change', appliedOn);
        wrapper.find('#location').first().simulate('change', {target: {value: location}});
        wrapper.find('#currency').first().simulate('change', {target: {value: currency}});
        wrapper.find('#typeOfPurchase').first().simulate('change', {target: {value: typeOfPurchase}});
        wrapper.find(`#${idFor('basicCustomsDuty')}`).first().simulate('change', basicCustomsDuty);
        wrapper.find(`#${idFor('locationVariance')}`).first().simulate('change',locationVariance);
        wrapper.find(`#${idFor('marketFluctuation')}`).first().simulate('change',  marketFluctuation);
        wrapper.find(`#${idFor('freightCharges')}`).first().simulate('change',  freightCharges);
        wrapper.find(`#${idFor('insuranceCharges')}`).first().simulate('change',  insuranceCharges);
        wrapper.find(`#${idFor('clearanceCharges')}`).first().simulate('change', clearanceCharges);
        wrapper.find(`#${idFor('taxVariance')}`).first().simulate('change', taxVariance);
        wrapper.find('#addForm').first().simulate('click');
        expect(wrapper.find(`#${idFor("Applied from")}`).first()).to.have.className('error');
    });

    it('should show mandatory fields when location is not filled',()=>{
        CreateRateRewired.__Rewire__('styles', {error: 'error'});
        shallow(wrapper.find('CollapsePanel').prop('header')).find('#addNew').simulate('click');
        wrapper.find(`#${idFor("Control Base Rate")}`).first().simulate('change', controlBaseRate);
        wrapper.find(`#${idFor("Applied from")}`).first().simulate('change', appliedOn);
        // wrapper.find('#location').first().simulate('change', {target: {value: location}});
        wrapper.find('#currency').first().simulate('change', {target: {value: currency}});
        wrapper.find('#typeOfPurchase').first().simulate('change', {target: {value: typeOfPurchase}});
        wrapper.find(`#${idFor('basicCustomsDuty')}`).first().simulate('change', basicCustomsDuty);
        wrapper.find(`#${idFor('locationVariance')}`).first().simulate('change',locationVariance);
        wrapper.find(`#${idFor('marketFluctuation')}`).first().simulate('change',  marketFluctuation);
        wrapper.find(`#${idFor('freightCharges')}`).first().simulate('change',  freightCharges);
        wrapper.find(`#${idFor('insuranceCharges')}`).first().simulate('change',  insuranceCharges);
        wrapper.find(`#${idFor('clearanceCharges')}`).first().simulate('change', clearanceCharges);
        wrapper.find(`#${idFor('Tax Variance')}`).first().simulate('change', taxVariance);
        wrapper.find('#addForm').first().simulate('click');
        expect(wrapper.find(`#location`).first()).to.have.className('error');
    });

    it('should show mandatory fields when typeOfPurchase is not filled',()=>{
        CreateRateRewired.__Rewire__('styles', {error: 'error'});
        shallow(wrapper.find('CollapsePanel').prop('header')).find('#addNew').simulate('click');
        wrapper.find(`#${idFor("Control Base Rate")}`).first().simulate('change', controlBaseRate);
        wrapper.find(`#${idFor("Applied from")}`).first().simulate('change', appliedOn);
        wrapper.find('#location').first().simulate('change', {target: {value: location}});
        wrapper.find('#currency').first().simulate('change', {target: {value: currency}});
        // wrapper.find('#typeOfPurchase').first().simulate('change', {target: {value: typeOfPurchase}});
        wrapper.find(`#${idFor('basicCustomsDuty')}`).first().simulate('change', basicCustomsDuty);
        wrapper.find(`#${idFor('locationVariance')}`).first().simulate('change',locationVariance);
        wrapper.find(`#${idFor('marketFluctuation')}`).first().simulate('change',  marketFluctuation);
        wrapper.find(`#${idFor('freightCharges')}`).first().simulate('change',  freightCharges);
        wrapper.find(`#${idFor('insuranceCharges')}`).first().simulate('change',  insuranceCharges);
        wrapper.find(`#${idFor('clearanceCharges')}`).first().simulate('change', clearanceCharges);
        wrapper.find(`#${idFor('taxVariance')}`).first().simulate('change', taxVariance);
        wrapper.find('#addForm').first().simulate('click');
        expect(wrapper.find(`#typeOfPurchase`).first()).to.have.className('error');
    });

    it('should show mandatory fields when currency is not filled',()=>{
        CreateRateRewired.__Rewire__('styles', {error: 'error'});
        shallow(wrapper.find('CollapsePanel').prop('header')).find('#addNew').simulate('click');
        wrapper.find(`#${idFor("Control Base Rate")}`).first().simulate('change', controlBaseRate);
        wrapper.find(`#${idFor("Applied from")}`).first().simulate('change', appliedOn);
        wrapper.find('#location').first().simulate('change', {target: {value: location}});
        wrapper.find('#currency').first().simulate('change', {target: {value: ''}});
        wrapper.find('#typeOfPurchase').first().simulate('change', {target: {value: typeOfPurchase}});
        wrapper.find(`#${idFor('basicCustomsDuty')}`).first().simulate('change', basicCustomsDuty);
        wrapper.find(`#${idFor('locationVariance')}`).first().simulate('change',locationVariance);
        wrapper.find(`#${idFor('marketFluctuation')}`).first().simulate('change',  marketFluctuation);
        wrapper.find(`#${idFor('freightCharges')}`).first().simulate('change',  freightCharges);
        wrapper.find(`#${idFor('insuranceCharges')}`).first().simulate('change',  insuranceCharges);
        wrapper.find(`#${idFor('clearanceCharges')}`).first().simulate('change', clearanceCharges);
        wrapper.find(`#${idFor('taxVariance')}`).first().simulate('change', taxVariance);
        wrapper.find('#addForm').first().simulate('click');
        expect(wrapper.find(`#currency`).first()).to.have.className('error');
    });

    it('should error when multiple . are passed in control base rate',()=>{
        CreateRateRewired.__Rewire__('styles', {error: 'error'});
        shallow(wrapper.find('CollapsePanel').prop('header')).find('#addNew').simulate('click');
        wrapper.find(`#${idFor("Control Base Rate")}`).first().simulate('change', '1.0.0');
        wrapper.find(`#${idFor("Applied from")}`).first().simulate('change', appliedOn);
        wrapper.find('#location').first().simulate('change', {target: {value: location}});
        wrapper.find('#currency').first().simulate('change', {target: {value: currency}});
        wrapper.find('#typeOfPurchase').first().simulate('change', {target: {value: typeOfPurchase}});
        wrapper.find(`#${idFor('basicCustomsDuty')}`).first().simulate('change', basicCustomsDuty);
        wrapper.find(`#${idFor('locationVariance')}`).first().simulate('change',locationVariance);
        wrapper.find(`#${idFor('marketFluctuation')}`).first().simulate('change',  marketFluctuation);
        wrapper.find(`#${idFor('freightCharges')}`).first().simulate('change',  freightCharges);
        wrapper.find(`#${idFor('insuranceCharges')}`).first().simulate('change',  insuranceCharges);
        wrapper.find(`#${idFor('clearanceCharges')}`).first().simulate('change', clearanceCharges);
        wrapper.find(`#${idFor('taxVariance')}`).first().simulate('change', taxVariance);
        wrapper.find('#addForm').first().simulate('click');
        expect(wrapper.find(`#${idFor("Control Base Rate")}`).first()).to.have.className('error');
    });

    it('should not call add rate and keep values save when cancel selected', () => {

        shallow(wrapper.find('CollapsePanel').prop('header')).find('#addNew').simulate('click');
        wrapper.find(`#${idFor("Control Base Rate")}`).first().simulate('change', controlBaseRate);
        wrapper.find(`#${idFor("Applied from")}`).first().simulate('change', appliedOn);
        wrapper.find('#location').first().simulate('change', {target: {value: location}});
        wrapper.find('#currency').first().simulate('change', {target: {value: currency}});
        wrapper.find('#typeOfPurchase').first().simulate('change', {target: {value: typeOfPurchase}});
        wrapper.find(`#${idFor('basicCustomsDuty')}`).first().simulate('change', basicCustomsDuty);
        wrapper.find(`#${idFor('locationVariance')}`).first().simulate('change',locationVariance);
        wrapper.find(`#${idFor('marketFluctuation')}`).first().simulate('change',  marketFluctuation);
        wrapper.find(`#${idFor('freightCharges')}`).first().simulate('change',  freightCharges);
        wrapper.find(`#${idFor('insuranceCharges')}`).first().simulate('change',  insuranceCharges);
        wrapper.find(`#${idFor('clearanceCharges')}`).first().simulate('change', clearanceCharges);
        wrapper.find(`#${idFor('taxVariance')}`).first().simulate('change', taxVariance);
        wrapper.find('#addForm').first().simulate('click');
        wrapper.find('#ConfirmationDialog').simulate('no');
        expect(wrapper.find(`#${idFor("Control Base Rate")}`).props('value').value).to.deep.equal(controlBaseRate);
        expect(wrapper.find(`#${idFor("Applied from")}`).props('selected').selected).to.not.be.empty;
        expect(wrapper.find('#location').props('value').value).to.deep.equal(location);
        expect(wrapper.find('#currency').props('value').value).to.deep.equal(currency);
        expect(wrapper.find('#typeOfPurchase').props('value').value).to.deep.equal(typeOfPurchase);
        expect(wrapper.find(`#${idFor('basicCustomsDuty')}`).props('value').value).to.deep.equal(basicCustomsDuty);
        expect(wrapper.find(`#${idFor('locationVariance')}`).props('value').value).to.deep.equal(locationVariance);
        expect(wrapper.find(`#${idFor('marketFluctuation')}`).props('value').value).to.deep.equal(marketFluctuation);
        expect(wrapper.find(`#${idFor('freightCharges')}`).props('value').value).to.deep.equal(freightCharges);
        expect(wrapper.find(`#${idFor('insuranceCharges')}`).props('value').value).to.deep.equal(insuranceCharges);
        expect(wrapper.find(`#${idFor('clearanceCharges')}`).props('value').value).to.deep.equal(clearanceCharges);
        expect(wrapper.find(`#${idFor('taxVariance')}`).props('value').value).to.deep.equal(taxVariance);
    });

    it('should show confirmation dialog box if one value is add',()=>{
        shallow(wrapper.find('CollapsePanel').prop('header')).find('#addNew').simulate('click');
        wrapper.find(`#${idFor("Control Base Rate")}`).first().simulate('change', controlBaseRate);
        wrapper.find("#cancelForm").simulate('click');
        expect(wrapper.find('#CancelConfirmationDialog').prop('shown')).to.deep.equal(true);
    });

    it('should collapse the panel if cancel is pressed and confirm dialog is pressed yes',()=>{
        shallow(wrapper.find('CollapsePanel').prop('header')).find('#addNew').simulate('click');
        wrapper.find(`#${idFor("Control Base Rate")}`).first().simulate('change', controlBaseRate);
        wrapper.find("#cancelForm").simulate('click');
        wrapper.find('#CancelConfirmationDialog').simulate('yes');
        expect(wrapper.find('Collapse').prop('activeKey')).to.deep.equal(null);
    });

    it('should not call onAddRate if rateAdding is true', ()=>{
        props.rateAdding = true;
        wrapper = shallow(<CreateRateActual {...props}/>);
        shallow(wrapper.find('CollapsePanel').prop('header')).find('#addNew').simulate('click');
        wrapper.find(`#${idFor("Control Base Rate")}`).first().simulate('change', controlBaseRate);
        wrapper.find(`#${idFor("Applied from")}`).first().simulate('change', appliedOn);
        wrapper.find('#location').first().simulate('change', {target: {value: location}});
        wrapper.find('#currency').first().simulate('change', {target: {value: currency}});
        wrapper.find('#typeOfPurchase').first().simulate('change', {target: {value: typeOfPurchase}});
        wrapper.find(`#${idFor('basicCustomsDuty')}`).first().simulate('change', basicCustomsDuty);
        wrapper.find(`#${idFor('locationVariance')}`).first().simulate('change',locationVariance);
        wrapper.find(`#${idFor('marketFluctuation')}`).first().simulate('change',  marketFluctuation);
        wrapper.find(`#${idFor('freightCharges')}`).first().simulate('change',  freightCharges);
        wrapper.find(`#${idFor('insuranceCharges')}`).first().simulate('change',  insuranceCharges);
        wrapper.find(`#${idFor('clearanceCharges')}`).first().simulate('change', clearanceCharges);
        wrapper.find(`#${idFor('taxVariance')}`).first().simulate('change', taxVariance);
        wrapper.find('#addForm').first().simulate('click');
        wrapper.find('#ConfirmationDialog').simulate('yes');
        expect(onAddRate).to.not.be.called;
    });

});

import React from 'react';
import {shallow} from 'enzyme';
import {CreateRentalRateActual, __RewireAPI__ as CreateRentalRateRewired} from '../../src/components/create-rental-rate';
import chai, {expect} from 'chai';
import chaiEnzyme from 'chai-enzyme';
import sinonChai from 'sinon-chai';
import sinon from 'sinon';
import {idFor} from '../../src/helpers';

chai.use(chaiEnzyme());
chai.use(sinonChai);

describe('CreateRentalRate',()=>{
    let wrapper, props, onAddRentalRate, onAddRentalRateErrorClose, onUnitOfMeasureFetch;
    const unitOfMeasure = 'Daily';
    const rentalRate = 1000;
    const appliedFrom = "03/04/2017";
    const currency = "INR";
    const expected={
        rentalRateValue:{
            value:rentalRate,
            currency:currency
        },
        unitOfMeasure:unitOfMeasure,
        appliedFrom:appliedFrom
    };
    beforeEach(()=>{
        onAddRentalRate = sinon.spy();
        onAddRentalRateErrorClose = sinon.spy();
        onUnitOfMeasureFetch = sinon.spy();
        props = {
            addRentalRateError:'',
            rentalRateAdding:false,
            onAddRentalRate:onAddRentalRate,
            componentCode:'CLY0001',
            onAddRentalRateErrorClose:onAddRentalRateErrorClose,
            rentalUnitData: {isFetching: false, values: {values: []}},
            currencyData: {isFetching: false, values: {values: []}},
        };
        wrapper = shallow(<CreateRentalRateActual {...props}/>);
    });
    it('should call onAddRentalRate when validate values are entered',()=>{
        shallow(wrapper.find('CollapsePanel').prop('header')).find('#addNew').simulate('click');
        wrapper.find("#unitOfMeasure").first().simulate('change', {target:{value:unitOfMeasure}});
        wrapper.find("#rentalRate").first().simulate('change', rentalRate);
        wrapper.find("#appliedFrom").first().simulate('change', appliedFrom);
        wrapper.find("#currency").first().simulate('change', {target:{value:currency}});
        wrapper.find("#addForm").simulate('click');
        wrapper.find("#addConfirmation").simulate('yes');
        expect(onAddRentalRate).to.be.calledWith('CLY0001',expected);

    });
    it('should show mandatory fields when rental is not filled',()=>{
        CreateRentalRateRewired.__Rewire__('styles', {error: 'error'});
        shallow(wrapper.find('CollapsePanel').prop('header')).find('#addNew').simulate('click');
        wrapper.find("#unitOfMeasure").first().simulate('change', {target:{value:unitOfMeasure}});
        // wrapper.find("#rentalRate").first().simulate('change', rentalRate);
        wrapper.find("#appliedFrom").first().simulate('change', appliedFrom);
        wrapper.find("#currency").first().simulate('change', {target:{value:currency}});
        wrapper.find("#addForm").simulate('click');
        expect(wrapper.find('#rentalRate').first()).to.have.className('error');
    });
    it('should show mandatory fields when unitOfMeasure is not filled',()=>{
        CreateRentalRateRewired.__Rewire__('styles', {error: 'error'});
        shallow(wrapper.find('CollapsePanel').prop('header')).find('#addNew').simulate('click');
        // wrapper.find("#unitOfMeasure").first().simulate('change', {target:{value:unitOfMeasure}});
        wrapper.find("#rentalRate").first().simulate('change', rentalRate);
        wrapper.find("#appliedFrom").first().simulate('change', appliedFrom);
        wrapper.find("#currency").first().simulate('change', {target:{value:currency}});
        wrapper.find("#addForm").simulate('click');
        expect(wrapper.find('#unitOfMeasure').first()).to.have.className('error');
    });
    it('should show mandatory fields when currency is not filled',()=>{
        CreateRentalRateRewired.__Rewire__('styles', {error: 'error'});
        shallow(wrapper.find('CollapsePanel').prop('header')).find('#addNew').simulate('click');
        wrapper.find("#unitOfMeasure").first().simulate('change', {target:{value:unitOfMeasure}});
        wrapper.find("#rentalRate").first().simulate('change', rentalRate);
        wrapper.find("#appliedFrom").first().simulate('change', appliedFrom);
        wrapper.find("#currency").first().simulate('change', {target:{value:''}});
        wrapper.find("#addForm").simulate('click');
        expect(wrapper.find('#currency').first()).to.have.className('error');
    });
    it('should show mandatory fields when appliedFrom is not filled',()=>{
        CreateRentalRateRewired.__Rewire__('styles', {error: 'error'});
        shallow(wrapper.find('CollapsePanel').prop('header')).find('#addNew').simulate('click');
        wrapper.find("#unitOfMeasure").first().simulate('change', {target:{value:unitOfMeasure}});
        wrapper.find("#rentalRate").first().simulate('change', rentalRate);
        // wrapper.find("#appliedFrom").first().simulate('change', appliedFrom);
        wrapper.find("#currency").first().simulate('change', {target:{value:currency}});
        wrapper.find("#addForm").simulate('click');
        expect(wrapper.find('#appliedFrom').first()).to.have.className('error');
    });
    it('should error when multiple . are passed in rentalRate',()=>{
        CreateRentalRateRewired.__Rewire__('styles', {error: 'error'});
        shallow(wrapper.find('CollapsePanel').prop('header')).find('#addNew').simulate('click');
        wrapper.find("#unitOfMeasure").first().simulate('change', {target:{value:unitOfMeasure}});
        wrapper.find("#rentalRate").first().simulate('change', '1.2.0');
        wrapper.find("#appliedFrom").first().simulate('change', appliedFrom);
        wrapper.find("#currency").first().simulate('change', {target:{value:currency}});
        wrapper.find("#addForm").simulate('click');
        expect(wrapper.find('#rentalRate').first()).to.have.className('error');
    });
    it('should not call add renral rate and keep values save when cancel selected',()=>{
        CreateRentalRateRewired.__Rewire__('styles', {error: 'error'});
        shallow(wrapper.find('CollapsePanel').prop('header')).find('#addNew').simulate('click');
        wrapper.find("#unitOfMeasure").first().simulate('change', {target:{value:unitOfMeasure}});
        wrapper.find("#rentalRate").first().simulate('change', rentalRate);
        wrapper.find("#appliedFrom").first().simulate('change', appliedFrom);
        wrapper.find("#currency").first().simulate('change', {target:{value:currency}});
        wrapper.find("#addForm").simulate('click');
        wrapper.find("#addConfirmation").simulate('no');
        expect(wrapper.find("#unitOfMeasure").props('value').value).to.deep.equal(unitOfMeasure);
        expect(wrapper.find("#rentalRate").props('value').value).to.deep.equal(rentalRate);
        expect(wrapper.find("#currency").props('value').value).to.deep.equal(currency);
        expect(wrapper.find("#appliedFrom").props('selected').selected).to.not.be.empty;
    });
    it('should show confirmation dialog box if one value is add',()=>{
        shallow(wrapper.find('CollapsePanel').prop('header')).find('#addNew').simulate('click');
        wrapper.find("#rentalRate").first().simulate('change', rentalRate);
        wrapper.find("#cancelForm").simulate('click');
        expect(wrapper.find('#cancelConfirmation').prop('shown')).to.deep.equal(true);
    });
    it('should collapse the panel if cancel is pressed and confirm dialog is pressed yes',()=>{
        shallow(wrapper.find('CollapsePanel').prop('header')).find('#addNew').simulate('click');
        wrapper.find(`#${idFor("rentalRate")}`).first().simulate('change', rentalRate);
        wrapper.find("#cancelForm").simulate('click');
        wrapper.find('#cancelConfirmation').simulate('yes');
        expect(wrapper.find('Collapse').prop('activeKey')).to.deep.equal(null);
    });
    it('should not call onAddRate if rateAdding is true',()=>{
        props.rentalRateAdding = true;
        wrapper = shallow(<CreateRentalRateActual {...props}/>);
        shallow(wrapper.find('CollapsePanel').prop('header')).find('#addNew').simulate('click');
        wrapper.find("#unitOfMeasure").first().simulate('change', {target:{value:unitOfMeasure}});
        wrapper.find("#rentalRate").first().simulate('change', rentalRate);
        wrapper.find("#appliedFrom").first().simulate('change', appliedFrom);
        wrapper.find("#currency").first().simulate('change', {target:{value:currency}});
        wrapper.find("#addForm").simulate('click');
        wrapper.find("#addConfirmation").simulate('yes');
        expect(onAddRentalRate).to.not.be.called
    })

});

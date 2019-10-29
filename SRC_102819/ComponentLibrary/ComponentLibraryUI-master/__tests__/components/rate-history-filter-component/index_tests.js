import React from 'react';
import sinon from 'sinon';
import sinonChai from 'sinon-chai';
import chai, { expect } from 'chai';
import { shallow } from 'enzyme';
import chaiEnzyme from 'chai-enzyme';
import { RateHistoryFilterComponent } from '../../../src/components/rate-history-filter-component';

chai.use(chaiEnzyme());
chai.use(sinonChai);


describe('#RateHistoryFilterComponent', () => {
	let filterDefinition =  {
			filterData: {typeOfPurchase: {
				name: 'Type of Purchase',
				value: '',
				id: 'typeOfPurchase',
				type: 'MasterData',
        data: {values: {values: ['INR']}}
      }, location: {
				name: 'Location',
				value: '',
				id: 'location',
				type: 'MasterData',
			}, appliedOn: {
				name: 'Applied On',
				value: null,
				id: 'appliedOn',
				type: 'Date'
			}}, error: {
				message: "",
				shown: false
			}
		};

  let filteredDefinition =  {
    filterData: {typeOfPurchase: {
      name: 'Type of Purchase',
      value: 'INR',
      id: 'typeOfPurchase',
      type: 'MasterData',
      data: {values: {values: ['INR']}}
    }, location: {
      name: 'Location',
      value: '',
      id: 'location',
      type: 'MasterData',
    }, appliedOn: {
      name: 'Applied On',
      value: null,
      id: 'appliedOn',
      type: 'Date'
    }}, error: {
      message: "",
      shown: false
    }
  };
	let closeDialogSpy = sinon.spy();
	let applyFilterSpy = sinon.spy();
	let clearFilterSpy = sinon.spy();
	let props = {
	  changedDefinition: filteredDefinition,
		componentType: 'material',
		definition: filterDefinition,
		isOpen: true,
		onClose: closeDialogSpy,
		onApply: applyFilterSpy,
		onClear: clearFilterSpy
	};
	let wrapper = shallow(<RateHistoryFilterComponent {...props} />);
	it('should trigger handleApply on clicking filter button', () => {
		wrapper.find('#apply').at(0).simulate('click');
		expect(applyFilterSpy).to.be.called;
	});

	it('should trigger onClear on clicking filter button', () => {
		wrapper.find('#clear').at(0).simulate('click');
		expect(clearFilterSpy).to.be.called;
	});

	it('should trigger onClose on clicking filter button', () => {
		wrapper.find('#close').at(0).simulate('click');
		expect(closeDialogSpy).to.be.called;
	});

	it('should render MasterData with columnValue when type is master data', () => {
	  expect(wrapper).to.have.descendants('MasterData');
	  expect(wrapper.find('dt').first()).to.have.text('Type of Purchase');
	  const expected = {
	    value: '',
      editable: true,
      dataType: {values: {values: ['INR']}}
    };
	  expect(wrapper.find('MasterData').first()).to.have.prop('columnValue').to.eql(expected);
  });

	it('should render DatePicker when type is Date', () => {
    expect(wrapper).to.have.descendants('DatePicker');
    expect(wrapper.find('dt').at(2)).to.have.text('Applied On');
  });
});


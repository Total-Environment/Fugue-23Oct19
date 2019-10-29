import React from 'react';
import sinon from 'sinon';
import sinonChai from 'sinon-chai';
import chai, {expect} from 'chai';
import {shallow} from 'enzyme';
import chaiEnzyme from 'chai-enzyme';
import {Filter} from '../../../src/components/filter';

chai.use(chaiEnzyme());
chai.use(sinonChai);


describe('FilterComponent', () => {
  let columnsList;
  let filterChangeSpy;
  let closeDialogSpy;
  let applyFilterSpy;
  let resetFilter;
  let props;
  let wrapper;

  beforeEach(() => {
    columnsList = [{
      name: 'Classification',
      columns: [{
        name: 'Material Level 1',
        key: 'material_level_1',
        editable: true,
        dataType: {
          name: 'MasterData',
          value: '',
          values: {
            values: [
              'Aluminium and Copper',
              'Test Material',
              'Test Material #2'
            ]
          }
        },
        validity: {
          isValid: true, msg: ''
        }
      }]
    }, {
      name: 'Others',
      columns: [{
        name: 'Status',
        key: 'status',
        editable: true,
        validity: {
          isValid: true, msg: ''
        },
        value: '',
        dataType: {
          name: 'MasterData',
          value: '',
          values: {
            values: [
              'Approved',
              'Inactive'
            ]
          }
        }
      }]
    }];
    filterChangeSpy = sinon.spy();
    closeDialogSpy = sinon.spy();
    applyFilterSpy = sinon.spy();
    resetFilter = sinon.spy();
    props = {
      filters: [{"columnKey":"material_status","columnValue":"Approved"},{"columnKey":"Location","columnValue":"Bengaluru"},{"columnKey":"AppliedOn","columnValue":"2017-05-25T10:09:00.204Z"}],
      onFilterChange: filterChangeSpy,
      isOpen: true,
      closeDialog: closeDialogSpy,
      applyFilter: applyFilterSpy,
      componentType: 'material',
      resetFilter: resetFilter,
      classificationData: {
        values: {
          block: {
            children: [{
              children: [],
              name: 'primary'
            },
              {
                children: [],
                name: 'secondary'
              }]
            , name: 'dependency'
          }, columnList: ['material level 1'], name: "materialClassifications"
        }, isFetching: false
      },
      masterData: {'material_status': {id: 'id', name: 'material_status', values: { values:["inactive,active"]}},
      'status':{id: 'id', name: 'status', values:{ values: ["approved,inactive"]}},
        'type_of_purchase':{id: 'id', name: 'type_of_purchase', values:{ values: ["approved,inactive"]}},
        'location':{id: 'id', name: 'location', values:{ values: ["approved,inactive"]}}},
    };
    wrapper = shallow(<Filter {...props} />);
  });

  it('should trigger applyFilter on clicking filter button', () => {
    wrapper.find('#apply').at(0).simulate('click', {preventDefault: sinon.spy()});
    expect(applyFilterSpy).to.be.called;
  });

  it('should trigger clearFilter on clicking filter button', () => {
    wrapper.find('#clear').at(0).simulate('click');
    expect(resetFilter).to.be.called;
  });

  it('should trigger closeDialog on clicking filter button', () => {
    wrapper.find('#close').at(0).simulate('click');
    expect(closeDialogSpy).to.be.called;
  });

  it('should render two tabs "Classification" and "Others"', () => {
    expect(wrapper.find('Tab')).to.have.length(2);
  });
});


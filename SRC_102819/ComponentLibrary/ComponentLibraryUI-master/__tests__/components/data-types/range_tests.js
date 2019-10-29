import chai, {expect} from 'chai';
import React from 'react';
import {shallow} from 'enzyme';
import chaiEnzyme from 'chai-enzyme';
import {Range} from '../../../src/components/data-types/range';
import {idFor} from '../../../src/helpers';
import sinon from 'sinon';
import sinonChai from 'sinon-chai';

chai.use(sinonChai);
chai.use(chaiEnzyme());

describe('Range', () => {
  let props, wrapper;

  beforeEach(() => {
    props = {
      columnName: "moisture Content", columnValue: {
        "value": {
          "from": 5,
          "to": 15,
          "unit": "%"
        },
        "dataType": {
          "name": "Range",
          "subType": "%"
        }
      }
    };
    wrapper = shallow(<Range {...props}/>);
  });

  it('should render data when to and from are available', () => {
    expect(wrapper).to.have.text('5-15%');
  });

  it('should render data when only from is available', () => {
    props.columnValue.value.to = null;
    wrapper = shallow(<Range {...props}/>);
    expect(wrapper).to.have.text('5%');
  });

  it('should render Decimal when filterable and editable is true', () => {
    props.columnValue = {"value": 23,
    "dataType": {
      "name": "Range",
      "subType": "%"
    },
    filterable:true,
    editable: true};
    wrapper = shallow(<Range {...props}/>);
    expect(wrapper).to.have.descendants('Decimal');
    expect(wrapper.find('Decimal')).to.have.prop('columnValue').deep.equal({"value": 23,
      "dataType": {
      "name": "Decimal",
        "subType": null
    },
      filterable:true,
    editable:true});
  });

  it('should take input when editable is true', () => {
      props = {
        columnName: "specific Gravity", columnValue: {
          "value": null,
          "dataType": {
            "name": "Range",
            "subType": "%"
          },
          validity: {isValid: true, msg: ''},
          editable: true,
        }
      };
      wrapper = shallow(<Range {...props}/>);
      expect(wrapper).to.have.exactly(2).descendants('InputNumber');
    }
  );

  describe('onChange', () => {
    beforeEach(() => {
      props = {
        columnName: "specific Gravity", columnValue: {
          "value": null,
          "dataType": {
            "name": "Range",
            "subType": "%"
          },
          editable: true,
          validity: {isValid: true, msg: ''}
        },
        onChange: sinon.spy(),
      };
      wrapper = shallow(<Range {...props}/>);
    });
    it('should trigger onChange when value is null and first input changes', () => {
      wrapper.find('InputNumber').first().simulate('change', 1);
      expect(props.onChange).to.have.been.calledWith({from: 1, to: null, unit: '%'});
    });
    it('should trigger onChange when value exists and first input changes', () => {
      props.columnValue.value = {from: 2, to: 2};
      wrapper = shallow(<Range {...props}/>);
      wrapper.find('InputNumber').first().simulate('change', 1);
      expect(props.onChange).to.have.been.calledWith({from: 1, to: 2, unit: '%'});
    });
    it('should trigger onChange when value is null and first input changes', () => {
      wrapper.find('InputNumber').first().simulate('change', 1);
      expect(props.onChange).to.have.been.calledWith({from: 1, to: null, unit: '%'});
    });
    it('should trigger onChange when value exists and first input changes', () => {
      props.columnValue.value = {from: 2, to: 2};
      wrapper = shallow(<Range {...props}/>);
      wrapper.find('InputNumber').at(1).simulate('change', 1);
      expect(props.onChange).to.have.been.calledWith({from: 2, to: 1, unit: '%'});
    });
    it('should trigger onChange null when from is null and from changes', () => {
      props.columnValue.value = {from: 2, to: 2};
      wrapper = shallow(<Range {...props}/>);
      wrapper.find('InputNumber').first().simulate('change', null);
      expect(props.onChange).to.have.been.calledWith(null);
    });
    it('should trigger onChange null when from is null and to changes', () => {
      props.columnValue.value = {from: null, to: 2};
      wrapper = shallow(<Range {...props}/>);
      wrapper.find('InputNumber').at(1).simulate('change', 3);
      expect(props.onChange).to.have.been.calledWith(null);
    });
  });
});

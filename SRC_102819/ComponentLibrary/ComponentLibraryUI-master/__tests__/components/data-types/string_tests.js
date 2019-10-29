import {shallow} from 'enzyme';
import chai, {expect} from 'chai';
import React from 'react';
import chaiEnzyme from 'chai-enzyme';
import {String} from '../../../src/components/data-types/string';
import sinon from 'sinon';
import sinonChai from 'sinon-chai';

chai.use(sinonChai);
chai.use(chaiEnzyme());

describe('String', () => {
  it('should render ReadMore with children as column value', () => {
    const props = {
      columnName: "short Description", columnValue: {
        "value": "Plywood General Purpose MR Grade 4mm",
        "dataType": {
          "name": "String",
          "subType": null
        },
        validity: {isValid: true, msg: ""}
      }
    };
    const wrapper = shallow(<String {...props}/>);
    expect(wrapper).to.have.descendants('ReadMore');
    expect(wrapper.find('ReadMore')).to.have.prop('children', props.columnValue.value);
  });

  it('should render ReadMore with children as column value array if it contains newlines', () => {
    const props = {
      columnName: "short Description", columnValue: {
        "value": "Plywood General\n Purpose MR Grade 4mm",
        "dataType": {
          "name": "String",
          "subType": null
        },
        validity: {isValid: true, msg: ""}
      }
    };
    const wrapper = shallow(<String {...props}/>);
    expect(wrapper).to.have.descendants('ReadMore');
    expect(wrapper.find('ReadMore')).to.have.prop('children').eql(['Plywood General', <br/>, ' Purpose MR Grade 4mm']);
  });

  it('should take textarea for a description type when editable is true', () => {
    const props = {
      columnName: "short Description", columnValue: {
        "value": "Sattar",
        "dataType": {
          "name": "String",
          "subType": null
        },
        editable: true,
        validity: {isValid: true, msg: ""}
      }
    };
    const wrapper = shallow(<String {...props}/>);
    expect(wrapper).to.have.descendants('textarea');
    expect(wrapper.find('textarea')).prop('value').to.equal('Sattar');
  });

  it('should render input when value is null', () => {
    const props = {
      columnName: "Some Column",
      columnValue: {
        "value": null,
        "dataType": {
          "name": "String",
          "subType": null
        },
        editable: true,
        validity: {isValid: true, msg: ""}
      }
    };
    const wrapper = shallow(<String {...props}/>);
    expect(wrapper).to.have.descendants('input');
    expect(wrapper.find('input')).prop('value').to.equal('');
  });

  it('should take input when editable is true', () => {
    const props = {
      columnName: "material Name", columnValue: {
        "value": null,
        "dataType": {
          "name": "String",
          "subType": null
        },
        editable: true,
        validity: {isValid: true, msg: ""}
      }
    };
    const wrapper = shallow(<String {...props}/>);
    expect(wrapper).to.have.descendants('input');
  });

  it('should call on change with proper value', () => {
    let props = {
      columnName: "Sample",
      columnValue: {
        value: null,
        editable: true,
        dataType: {
          name: "String",
          subType: null
        },
        validity: {isValid: true, msg: ""}
      },
      onChange: sinon.spy()
    };
    const wrapper = shallow(<String {...props}/>);
    expect(wrapper).to.have.descendants('input');
    wrapper.find('input').simulate('change', {target: {value: 'New Value is Added'}});
    expect(props.onChange).to.have.been.calledWith('New Value is Added');
  });
  it('should call on change with proper value when it is description or additional features', () => {
    ['short Description', 'additional Features'].forEach((columnName) => {
      let props = {
        columnName: columnName,
        columnValue: {
          value: null,
          editable: true,
          dataType: {
            name: "String",
            subType: null
          },
          validity: {isValid: true, msg: ""}
        },
        onChange: sinon.spy()
      };
      const wrapper = shallow(<String {...props}/>);
      wrapper.find('textarea').simulate('change', {target: {value: 'New Value is Added'}});
      expect(props.onChange).to.have.been.calledWith('New Value is Added');
    });
  });
});

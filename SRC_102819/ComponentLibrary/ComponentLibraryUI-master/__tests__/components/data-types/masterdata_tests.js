import chai, {expect} from 'chai';
import React from 'react';
import {shallow} from 'enzyme';
import chaiEnzyme from 'chai-enzyme';
import {MasterData} from '../../../src/components/data-types/master-data';
import sinon from 'sinon';
import sinonChai from 'sinon-chai';

chai.use(sinonChai);
chai.use(chaiEnzyme());

describe('MasterData', () => {
  describe('render', () => {
    let props;
    beforeEach(() => {
      props = {
        columnName: 'Masterdata',
        columnValue: {
          value: null,
          editable: true,
          dataType: {
            name: "MasterData",
            subType: '123',
            values: {values: ['a', 'b', 'c', 'd'], status: 'fetched'}
          },
          validity: {isValid: true, msg: ""},
        },
        onChange: sinon.spy()
      };
    });
    it('should render a select when it is editable', () => {
      const wrapper = shallow(<MasterData {...props}/>);
      expect(wrapper).to.have.exactly(5).descendants('option');
    });

    it('should render data when it is not editable', () => {
      const props = {columnName: 'Masterdata', columnValue: {value: "MasterValue"}};
      const wrapper = shallow(<MasterData {...props}/>);
      expect(wrapper).to.have.text('MasterValue');
    });

    it('should render Material status if column status', () => {
      ['material Status', 'service Status', 'status'].forEach((name) => {
        const props = {columnName: name, columnValue: {value: "MasterValue"}};
        const wrapper = shallow(<MasterData {...props}/>);
        expect(wrapper).to.have.descendants('ComponentStatus');
        expect(wrapper.find('ComponentStatus')).to.have.prop('value').to.equal('MasterValue');
      });
    });

    it('should call onChange when select changes', () => {
      const wrapper = shallow(<MasterData {...props}/>);
      wrapper.find('select').simulate('change', {target: {value: 'b'}});
      expect(props.onChange).to.have.been.calledWith('b');
    });

    it('should render an extra option saying select', () => {
      const wrapper = shallow(<MasterData {...props}/>);
      expect(wrapper).to.include.text('--Select--');
    });

    it('should render input when column value has filterable true', () => {
      props = {
        columnName: 'Masterdata',
        columnValue: {
          value: null,
          editable: true,
          filterable:true,
          dataType: {
            name: "MasterData",
            subType: '123',
            values: {values: ['a', 'b', 'c', 'd'], status: 'fetched'}
          },
          validity: {isValid: true, msg: ""},
        },
        onChange: sinon.spy()
      };
      const wrapper = shallow(<MasterData {...props}/>);
      expect(wrapper).to.have.descendants('input');
      wrapper.find('input').simulate('change', {target: {value: 'new value'}});
      expect(props.onChange).to.have.been.calledWith('new value');
    });

    it('should render input when column value contains name value pair', () => {
      props = {
        columnName: 'Masterdata',
        columnValue: {
          value: null,
          editable: true,
          filterable:true,
          dataType: {
            name: "MasterData",
            subType: '123',
            values: {values: [{name: 'a', value: '1'}, 'b', 'c', 'd'], status: 'fetched'}
          },
          validity: {isValid: true, msg: ""},
        },
        onChange: sinon.spy()
      };
      const wrapper = shallow(<MasterData {...props}/>);
      expect(wrapper).to.have.descendants('input');
    });
  })
});

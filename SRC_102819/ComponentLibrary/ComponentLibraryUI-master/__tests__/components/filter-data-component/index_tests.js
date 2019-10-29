import React from 'react';
import {shallow} from "enzyme";
import chai, {expect} from 'chai';
import {FilterDataComponent} from '../../../src/components/filter-data-component'

describe('filter data component', () => {
  describe('render', () => {
    let props;
    beforeEach(() => {
      props = {definition : {
        headers: [
          {
            columns: [
              {
                name: 'Material Level 1',
                key: 'material_level_1',
                dataType: {name: 'MasterData', subType: 'MasterDataId'}, value: null,
                isRequired: false,
                editable: true,
                filterable: true,
                validity: {isValid: true, msg: ''}
              }
            ],
            name: 'Classification',
            key: 'classification'
          },
          {
            columns: [
              {
                name: 'HSN Code',
                key: 'hsn_code',
                dataType: {name: 'String', subType: null},
                value: null,
                isRequired: false,
                editable: true,
                filterable: true,
                validity: {isValid: true, msg: ''}
              }
            ],
            name: 'General',
            key: 'general'
          }
        ]
      },
        componentType:'material',
        group:'clay'}
    });
    it('should render headers when definition is present',()=>{
      const wrapper = shallow(<FilterDataComponent {...props}/>);
      expect(wrapper.find('Tab').first().children()).to.have.text('Classification');
      expect(wrapper.find('Tab').at(1).children()).to.have.text('General');
    });

    it('should render data type by sending props group column name and component type when definition is present', () => {
      const wrapper = shallow(<FilterDataComponent {...props}/>);
      expect(wrapper).to.have.descendants('DataType');
      expect(wrapper.find('DataType').first()).to.have.prop('columnName');
      expect(wrapper.find('DataType').first()).to.have.prop('columnName').equal('material_level_1');
      expect(wrapper.find('DataType').first()).to.have.prop('group');
      expect(wrapper.find('DataType').first()).to.have.prop('group').equal('clay');
      expect(wrapper.find('DataType').first()).to.have.prop('componentType');
      expect(wrapper.find('DataType').first()).to.have.prop('componentType').equal('material');
    });

    it('should call tabs with index of new header when header got changed', () => {
      const wrapper = shallow(<FilterDataComponent {...props}/>);
      wrapper.find('Tabs').simulate('select',1);
      expect(wrapper.find('Tabs')).to.have.prop('selectedIndex').equal(1);
    });
  });
});


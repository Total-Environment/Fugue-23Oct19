import {shallow} from 'enzyme';
import chai, {expect} from 'chai';
import React from 'react';
import chaiEnzyme from 'chai-enzyme';
import {Brand, AddBrand} from '../../../src/components/data-types/brand';

chai.use(chaiEnzyme());

describe('Brand', () => {

  function getBrandTypeProps() {
    return {
      columnName: "associated Brands",
      columnValue: {
        "value": {
          columns: [
          {key: 'brand_code', name: 'Brand Code', value: 'BSY001'},
          {key: 'manufacturers_name', name: "Manufacturer's Name", value: 'Jindal'},
        ]},
        "dataType": {
          "name": "Brand",
          "subType": null
        },
        validity: {isValid: true, msg: ""}
      }
    };
  }

  it('should render empty div when editable is true', () => {
    const wrapper = shallow(<Brand columnValue={{editable: true}} />);
    expect(wrapper.find('div')).to.have.length(1);
  });

  it('should contain contextTypes attached to Brand', () => {
    expect(Brand.contextTypes).to.be.a('object');
    expect(Brand.contextTypes).to.have.property('componentCode');
    expect(Brand.contextTypes).to.have.property('componentType');
  });

  it('should render Brand as link', () => {
    const props = getBrandTypeProps();
    const wrapper = shallow(<Brand {...props}/>, {context: {componentCode: 'ALM0001', componentType: 'material'}});
    expect(wrapper).to.have.descendants('a');
    const link = wrapper.find('a');
    expect(link.children()).to.have.text('BSY001');
    expect(link).to.have.prop('href', `/materials/ALM0001/brands/BSY001`);
  });

  it('should contain contextTypes attached to AddBrand', () => {
    expect(AddBrand.contextTypes).to.be.a('object');
    expect(AddBrand.contextTypes).to.have.property('componentCode');
    expect(AddBrand.contextTypes).to.have.property('componentType');
  });

  it('should render AddBrand as link', () => {
    const props = getBrandTypeProps();
    const wrapper = shallow(<AddBrand {...props} />, { context: { componentCode: 'ALM0001', componentType: 'material'}});
    expect(wrapper).to.have.descendants('a');
    const link = wrapper.find('a');
    expect(link.children()).to.have.text('Add New');
    expect(link).to.have.prop('href', `/materials/ALM0001/brands/new`);
  })
});

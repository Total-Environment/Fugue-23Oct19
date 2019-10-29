import React from 'react';
import sinon from 'sinon';
import sinonChai from 'sinon-chai';
import chai, { expect } from 'chai';
import { shallow } from 'enzyme';
import chaiEnzyme from 'chai-enzyme';
import { ComponentList } from '../../../src/components/composite-details/component-list';


chai.use(chaiEnzyme());
chai.use(sinonChai);

describe('#ComponentList', () => {
  let props;
  beforeEach(() => {
    props = {
      headers: [{
        key: 'general',
        name: 'General'
      }, {
        key: 'specifications',
        name: 'Specifications'
      }],
      onChooseComponents: sinon.spy(),
      onTriggerSearch: sinon.spy()
    };
  });

  it('should fetch proper component type, based on activeResource prop', () => {
    let newProps = Object.assign({}, props, {
      activeResource: 'asset',
      data: []
    });
    const wrapper = shallow(<ComponentList {...newProps} />);
    const instance = wrapper.instance();
    expect(instance._getActiveComponent()).to.be.equal('material');
  });

  it('should render tabsx2 with headers General and Specifications', () => {
    let newProps = Object.assign({}, props, {
      activeResource: 'material',
      data: {
        values: [],
        isFetching: false,
        componentType: 'material'
      }
    });
    const wrapper = shallow(<ComponentList {...newProps} />);
    expect(wrapper.find('Tab')).length(2);
    expect(wrapper.find('Tab').at(0).children()).to.have.text('General');
    expect(wrapper.find('Tab').at(1).children()).to.have.text('Specifications');
  });

  it('should render a loading spinner, when data is still being fetched', () => {
    let newProps = Object.assign({}, props, {
      activeResource: 'material',
      data: {
        values: [],
        isFetching: true,
        componentType: 'material'
      }
    });

    const wrapper = shallow(<ComponentList {...newProps} />);
    expect(wrapper.find('Loading')).to.exist;
  });

  it('should render result rows on valid data', () => {
    let newProps = Object.assign({}, props, {
      activeResource: 'material',
      data: {
        values: {
          items: [{
            headers: [
              {
                columns: [
                  {
                    name: 'Material Code',
                    key: 'material_code',
                    dataType: {
                      name: 'String',
                      subType: null
                    },
                    value: 'test1'
                  },
                  {
                    name: 'Image',
                    key: 'image',
                    value: ''
                  },
                  {
                    name: 'Material Name',
                    key: 'material_name',
                    dataType: {
                      name: 'String',
                      subType: null
                    },
                    value: 'mname'
                  },
                  {
                    name: 'Material Status',
                    key: 'material_status',
                    dataType: {
                      name: 'String',
                      subType: null
                    },
                    value: 'approved'
                  }
                ],
                name: 'General',
                key: 'general'
              }
            ]
          }]
        },
        isFetching: false,
        componentType: 'material'
      }
    });

    const wrapper = shallow(<ComponentList {...newProps} />);
    expect(wrapper.find('Loading')).to.not.exist;
    expect(wrapper.find('#general_test1')).to.have.length(1);
  });
});

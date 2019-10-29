import chai, {expect} from 'chai';
import React from 'react';
import {shallow} from 'enzyme';
import chaiEnzyme from 'chai-enzyme';
import {DateTime} from '../../../src/components/data-types/date-time';

chai.use(chaiEnzyme());

describe('Date', () => {
  it('should render date-time in proper format when given proper column value', () => {
    const props = {
      columnName: "date Created", columnValue: {
        "dataType": {
          "name": "DateTime",
          "subType": null
        },
        "value": "2017-01-25T07:47:08.155Z"
      }
    };
    const wrapper = shallow(<DateTime {...props}/>);
    expect(wrapper).to.have.text('25/1/2017');
  });
  it('should take input as date-time when editable is true', () => {
    const props = {
      columnName: "date Created", columnValue: {
        "dataType": {
          "name": "DateTime",
          "subType": null
        },
        "value": null,
        editable: true
      }
    };
    const wrapper = shallow(<DateTime {...props}/>);
    expect(wrapper).to.have.descendants('input[type="datetime-local"]');
  })
});
